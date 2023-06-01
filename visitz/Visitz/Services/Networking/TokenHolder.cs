using IdentityModel.OidcClient;
using IdentityModel.OidcClient.Results;
using System.IdentityModel.Tokens.Jwt;

namespace Visitz.Services.Networking
{
    public class TokenHolder
    {
        private static readonly string NamespaceKey = "visitz_oauth_";
        private static readonly string AccessTokenKey = NamespaceKey + "access_token";
        private static readonly string RefreshTokenKey = NamespaceKey + "refresh_token";

        public static JwtSecurityToken AccessToken { get; private set; }

        public static JwtSecurityToken RefreshToken { get; private set; }

        private async static Task SetAsync(string key, string value)
        {
            await SecureStorage.Default.SetAsync(key, value);
        }

        private async static Task<string> GetAsync(string key)
        {
            return await SecureStorage.Default.GetAsync(key);
        }

        private static async Task SaveAccessToken(string accessToken)
        {
            await SetAsync(AccessTokenKey, accessToken);
            AccessToken = new JwtSecurityToken(accessToken);
        }

        private static async Task SaveRefreshToken(string refreshToken)
        {
            await SetAsync(RefreshTokenKey, refreshToken);
            RefreshToken = new JwtSecurityToken(refreshToken);
        }

        public static async Task SaveAsync(LoginResult loginResult)
        {
            await SaveAccessToken(loginResult.AccessToken);
            await SaveRefreshToken(loginResult.RefreshToken);
        }

        public static async Task SaveAsync(RefreshTokenResult refreshResult)
        {
            await SaveAccessToken(refreshResult.AccessToken);
            await SaveRefreshToken(refreshResult.RefreshToken);
        }

        public static async Task<string> GetAccessTokenStringAsync()
        {
            return await GetAsync(AccessTokenKey);
        }
        public static async Task<string> GetRefreshTokenStringAsync()
        {
            return await GetAsync(RefreshTokenKey);
        }

        public static async Task<JwtSecurityToken> GetAccessTokenAsync()
        {
            if (await GetAccessTokenStringAsync() is string accessJwt)
                AccessToken ??= new JwtSecurityToken(accessJwt);

            return AccessToken;
        }

        public static async Task<JwtSecurityToken> GetRefreshTokenAsync()
        {
            if (await GetRefreshTokenStringAsync() is string refreshJwt)
                RefreshToken ??= new JwtSecurityToken(refreshJwt);

            return RefreshToken;
        }

        public static void DeleteAccessToken()
        {
            SecureStorage.Default.Remove(AccessTokenKey);
        }

        public static void DeleteRefreshToken()
        {
            SecureStorage.Default.Remove(RefreshTokenKey);
        }

        private static bool IsTokenValid(JwtSecurityToken token)
        {
            return DateTime.UtcNow < token?.ValidTo;
        }

        public static async Task<bool> IsAccessTokenValid()
        {
            if (DebugOptions.AlwaysExpireAccessToken)
                DeleteAccessToken();

            return IsTokenValid(await GetAccessTokenAsync());
        }

        public static async Task<bool> IsRefreshTokenExpired()
        {
            if (DebugOptions.AlwaysExpireRefreshToken)
                DeleteRefreshToken();

            return !IsTokenValid(await GetRefreshTokenAsync());
        }
    }
}

