using Visitz.Authentication.Keycloak.Events;

namespace Visitz.Authentication.Keycloak
{
    public class VisitzSession
    {
        private static AuthenticationClient AuthClient =>
            ServiceProvider.Current.GetRequiredService<AuthenticationClient>();

        private static bool InternetAvailable =>
            Connectivity.Current.NetworkAccess == NetworkAccess.Internet;

        public static event EventHandler<SessionChangedEventArgs> SessionChanged;

        public static async Task<bool> GetValidSessionAsync()
        {
            if (!InternetAvailable)
                return false;

            return await TokenHolder.IsAccessTokenValid()
                || await TryRefreshAsync()
                || await LoginAsync();
        }

        public static async Task<bool> LoginAsync()
        {
            var loginResult = await AuthClient.LoginAsync();
            var loginSuccess = !loginResult.IsError;

            if (loginSuccess)
                await TokenHolder.SaveAsync(loginResult);

            // TODO: Log errors.

            var info = await VisitzSessionInfo.GetAsync();
            SessionChanged?.Invoke(info, new LoginChangedEventArgs() { Success = loginSuccess });

            return loginSuccess;
        }

        private static async Task<bool> TryRefreshAsync()
        {
            if (await TokenHolder.IsRefreshTokenExpired())
                return false;

            return await RefreshAsync();
        }

        private static async Task<bool> RefreshAsync()
        {
            var refreshToken = await TokenHolder.GetRefreshTokenStringAsync();
            var refreshResult = await AuthClient.RefreshAsync(refreshToken);
            var refreshSuccess = !refreshResult.IsError;

            if (refreshSuccess)
                await TokenHolder.SaveAsync(refreshResult);

            // TODO: Log errors.

            var info = await VisitzSessionInfo.GetAsync();
            SessionChanged?.Invoke(info, new RefreshChangedEventArgs() { Success = refreshSuccess });

            return refreshSuccess;
        }

        public static async Task<bool> LogoutAsync()
        {
            var logoutResult = await AuthClient.LogoutAsync();
            var logoutSuccess = !logoutResult.IsError;

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
