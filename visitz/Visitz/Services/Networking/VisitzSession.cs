using Visitz.Services.Authentication;

namespace Visitz.Services.Networking
{
    public class VisitzSession
    {
        private static AuthenticationClient AuthClient => Application.Current.Handler.MauiContext
                .Services.GetRequiredService<AuthenticationClient>();

        public static async Task<bool> GetValidSessionAsync()
        {
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

        public static async Task<VisitzSessionInfo> GetInfoAsync()
        {
            return await VisitzSessionInfo.GetAsync();
        }
    }
}
