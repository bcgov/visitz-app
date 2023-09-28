using IdentityModel.OidcClient;
using IdentityModel.OidcClient.Results;
using Visitz.Authentication.Keycloak.Events;
using Visitz.Network;

namespace Visitz.Authentication.Keycloak
{
    public class VisitzSession
    {
        private static AuthenticationClient AuthClient =>
            ServiceProvider.Current.GetRequiredService<AuthenticationClient>();

        public static event EventHandler<SessionChangedEventArgs> SessionChanged;

        public static async Task AssertValidSessionAsync()
        {
            NetworkHelper.AssertInternetAvailable();

            if (!await TokenHolder.IsAccessTokenValid())
            {
                if (await TokenHolder.IsRefreshTokenExpired())
                    await LoginAsync();
                else
                    await RefreshAsync();
            }
        }

        public static async Task LoginAsync()
        {
            LoginResult loginResult = null;

            try
            {
                NetworkHelper.AssertInternetAvailable();

                loginResult = await AuthClient.LoginAsync();

                if (loginResult.IsError)
                    throw new LoginException(loginResult.Error);

                await TokenHolder.SaveAsync(loginResult);
            }
            finally
            {
#if DEBUG
                ConsoleTrace.TraceMethod(typeof(VisitzSession),
                    $"loginResult.IsError: '{loginResult?.IsError}', error: '{loginResult?.Error}'");
#endif
                var info = await VisitzSessionInfo.GetAsync();
                SessionChanged?.Invoke(info, new LoginChangedEventArgs()
                { 
                    Success = !loginResult?.IsError ?? false 
                });
            }
        }

        private static async Task RefreshAsync()
        {
            RefreshTokenResult refreshResult = null;

            try
            {
                NetworkHelper.AssertInternetAvailable();

                var refreshToken = await TokenHolder.GetRefreshTokenStringAsync();
                refreshResult = await AuthClient.RefreshAsync(refreshToken);

                if (refreshResult.IsError)
                    throw new SessionRefreshException(refreshResult.Error);

                await TokenHolder.SaveAsync(refreshResult);
            }
            finally
            {
#if DEBUG
                ConsoleTrace.TraceMethod(typeof(VisitzSession), 
                    $"refreshResult.IsError: '{refreshResult?.IsError}', error: '{refreshResult?.Error}'");
#endif
                var info = await VisitzSessionInfo.GetAsync();
                SessionChanged?.Invoke(info, new RefreshChangedEventArgs() 
                { 
                    Success = !refreshResult?.IsError ?? false 
                });
            }
        }

        public static async Task<bool> LogoutAsync()
        {
            var logoutResult = await AuthClient.LogoutAsync();
            var logoutSuccess = !logoutResult.IsError;

#if DEBUG
            ConsoleTrace.TraceMethod(typeof(VisitzSession),
                $"logoutResult.IsError: '{logoutResult.IsError}', error: '{logoutResult.Error}'");
#endif
            await InvalidateSessionAsync();

            var info = await VisitzSessionInfo.GetAsync();
            SessionChanged?.Invoke(info, new LogoutChangedEventArgs() { Success = logoutSuccess });

            return logoutSuccess;
        }

        public static async Task<VisitzSessionInfo> GetInfoAsync()
        {
            return await VisitzSessionInfo.GetAsync();
        }

        public static async Task InvalidateSessionAsync()
        {
            TokenHolder.DeleteAccessToken();
            TokenHolder.DeleteRefreshToken();
            TokenHolder.DeleteIdentityToken();

            var info = await VisitzSessionInfo.GetAsync();
            SessionChanged?.Invoke(info, new SessionInvalidatedEventArgs() { Success = true });
        }

        public static async Task<bool> SessionExistsAsync()
        {
            return await TokenHolder.GetAccessTokenStringAsync() is not null
                && await TokenHolder.GetRefreshTokenStringAsync() is not null
                && await TokenHolder.GetIdentityTokenStringAsync() is not null;
        }

        public static async Task<bool> HasBasicAccess()
        {
            var info = await VisitzSessionInfo.GetAsync();
            return await SessionExistsAsync() && info.HasBasicAccessRole;
        }
    }
}
