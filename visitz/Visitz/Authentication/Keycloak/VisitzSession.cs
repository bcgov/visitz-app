using IdentityModel.OidcClient.Browser;

namespace Visitz.Authentication.Keycloak
{
    public class VisitzSession
    {
        private static AuthenticationClient AuthClient =>
            ServiceProvider.Current.GetRequiredService<AuthenticationClient>();

        private static bool InternetAvailable =>
            Connectivity.Current.NetworkAccess == NetworkAccess.Internet;

        public static async Task<bool> GetValidSessionAsync()
        {
            if (!InternetAvailable)
                return false;

            return await TokenHolder.IsAccessTokenValid()
                || await TryRefreshAsync()
                || await LoginAsync();
        }

        private static async Task<bool> LoginAsync()
        {
            var loginResult = await AuthClient.LoginAsync();
            var loginSuccess = !loginResult.IsError;

            if (loginSuccess)
                await TokenHolder.SaveAsync(loginResult);

            // TODO: Log errors.

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

            return refreshSuccess;
        }

        public static async Task<bool> LogoutAsync()
        {
            var logoutResult = await AuthClient.LogoutAsync();
            var logoutSuccess = !logoutResult.IsError;

            InvalidateSession();

            return logoutSuccess;
        }

        public static async Task<VisitzSessionInfo> GetInfoAsync()
        {
            return await VisitzSessionInfo.GetAsync();
        }

        public static void InvalidateSession()
        {
            TokenHolder.DeleteAccessToken();
            TokenHolder.DeleteRefreshToken();
        }
    }
}
