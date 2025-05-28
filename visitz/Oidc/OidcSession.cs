using IdentityModel.OidcClient;
using IdentityModel.OidcClient.Browser;
using IdentityModel.OidcClient.Results;
using Oidc.Events;
using Oidc.Exceptions;
using Oidc.Network;
using Oidc.Util;

namespace Oidc
{
    public class OidcSession
    {
        private static readonly string IdirActiveKey = "idir_active_employee";

        private static AuthenticationClient AuthClient =>
            ServicesProvider.Current.GetRequiredService<AuthenticationClient>();

        public static event EventHandler<SessionChangedEventArgs> SessionChanged;

        public static async Task AssertValidSessionAsync(string messageIfUnavailable, CancellationToken cancellationToken = default)
        {
            NetworkHelper.AssertInternetAvailable(messageIfUnavailable);

            if (!await TokenHolder.IsAccessTokenValid())
            {
                if (await TokenHolder.IsRefreshTokenExpired())
                    await LoginAsync(messageIfUnavailable, cancellationToken);
                else
                    await RefreshAsync(messageIfUnavailable);
            }
        }

        public static async Task LoginAsync(string messageIfUnavailable, CancellationToken cancellationToken = default)
        {
            LoginResult loginResult = null;

            try
            {
                NetworkHelper.AssertInternetAvailable(messageIfUnavailable);

                loginResult = await AuthClient.LoginAsync(cancellationToken);

                if (loginResult.IsError)
                {
                    if (loginResult.Error == BrowserResultType.UserCancel.ToString())
                        // WORKAROUND String compare to BrowserResultType is a limitation of the library
                        throw new OperationCanceledException();
                    else
                        throw new LoginException($"{loginResult.Error}: '{loginResult.ErrorDescription}'");
                }

                await TokenHolder.SaveAsync(loginResult);
            }
            finally
            {
                bool success = !loginResult?.IsError ?? false;
#if DEBUG
                ConsoleTrace.TraceMethod(typeof(OidcSession),
                    $"Login success: '{success}', Error: {loginResult.Error} -> '{loginResult.ErrorDescription}'");
#endif
                var info = await OidcSessionInfo.GetAsync();
                SessionChanged?.Invoke(info, new LoginChangedEventArgs() { Success = success, });
            }
        }

        private static async Task RefreshAsync(string messageIfUnavailable)
        {
            RefreshTokenResult refreshResult = null;

            try
            {
                NetworkHelper.AssertInternetAvailable(messageIfUnavailable);

                var refreshToken = await TokenHolder.GetRefreshTokenStringAsync();
                refreshResult = await AuthClient.RefreshAsync(refreshToken);

                if (refreshResult.IsError)
                    throw new SessionRefreshException(refreshResult.Error);

                await TokenHolder.SaveAsync(refreshResult);
            }
            finally
            {
#if DEBUG
                ConsoleTrace.TraceMethod(typeof(OidcSession),
                    $"refreshResult.IsError: '{refreshResult?.IsError}', error: '{refreshResult?.Error}'");
#endif
                var info = await OidcSessionInfo.GetAsync();
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
            ConsoleTrace.TraceMethod(typeof(OidcSession),
                $"logoutResult.IsError: '{logoutResult.IsError}', error: '{logoutResult.Error}'");
#endif
            await InvalidateSessionAsync();

            var info = await OidcSessionInfo.GetAsync();
            SessionChanged?.Invoke(info, new LogoutChangedEventArgs() { Success = logoutSuccess });

            return logoutSuccess;
        }

        public static async Task LocalLogoutAsync()
        {
#if DEBUG
            ConsoleTrace.TraceMethod(typeof(OidcSession), $"Local logout initiated");
#endif
            await InvalidateSessionAsync();

            var info = await OidcSessionInfo.GetAsync();
            SessionChanged?.Invoke(info, new LogoutChangedEventArgs() { Success = true });
        }

        public static async Task<OidcSessionInfo> GetInfoAsync()
        {
            return await OidcSessionInfo.GetAsync();
        }

        public static async Task InvalidateSessionAsync()
        {
            TokenHolder.DeleteAccessToken();
            TokenHolder.DeleteRefreshToken();
            TokenHolder.DeleteIdentityToken();

            var info = await OidcSessionInfo.GetAsync();
            SessionChanged?.Invoke(info, new SessionInvalidatedEventArgs() { Success = true });
        }

        public static async Task<bool> SessionExistsAsync()
        {
            return await TokenHolder.GetAccessTokenStringAsync() is not null
                && await TokenHolder.GetRefreshTokenStringAsync() is not null
                && await TokenHolder.GetIdentityTokenStringAsync() is not null;
        }

        public static async Task<bool> IsAuthorized()
        {
            var status = await SecureStorage.Default.GetAsync(IdirActiveKey);
            return status != null && bool.Parse(status);
        }

        public static async Task SetAuthorization(bool authorized)
        {
            await SecureStorage.Default.SetAsync(IdirActiveKey, authorized.ToString());
        }
    }
}
