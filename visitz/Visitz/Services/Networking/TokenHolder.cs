using IdentityModel.OidcClient;
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

        public static async Task SaveAsync(LoginResult loginResult)
        {
            await SetAsync(AccessTokenKey, loginResult.AccessToken);
            await SetAsync(RefreshTokenKey, loginResult.RefreshToken);

            AccessToken = new JwtSecurityToken(loginResult.AccessToken);
            RefreshToken = new JwtSecurityToken(loginResult.RefreshToken);
        }

        public static async Task<string> GetAccessTokenStringAsync()
        {
            return await GetAsync(AccessTokenKey);
        }

        public static async Task<JwtSecurityToken> GetAccessTokenAsync()
        {
            if (await GetAccessTokenStringAsync() is string accessJwt)
                AccessToken ??= new JwtSecurityToken(accessJwt);

            return AccessToken;
        }

        public static async Task<JwtSecurityToken> GetRefreshTokenAsync()
        {
            if (await GetAsync(RefreshTokenKey) is string refreshJwt)
                RefreshToken ??= new JwtSecurityToken(refreshJwt);

            return RefreshToken;
        }

        public static void Delete()
        {
            SecureStorage.Default.Remove(AccessTokenKey);
            SecureStorage.Default.Remove(RefreshTokenKey);
        }

        private static bool IsTokenValid(JwtSecurityToken token)
        {
            return token.ValidTo < DateTime.UtcNow;
        }

        public static async Task<bool> IsAccessTokenValid()
        {
            return IsTokenValid(await GetAccessTokenAsync());
        }

        public static async Task<bool> IsRefreshTokenValid()
        {
            return IsTokenValid(await GetRefreshTokenAsync());
        }
    }
}

