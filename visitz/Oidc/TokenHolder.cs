using System.IdentityModel.Tokens.Jwt;
using IdentityModel.OidcClient;
using IdentityModel.OidcClient.Results;

namespace Oidc;

public class TokenHolder
{
    // TODO: Allow this namespace prefix to be configurable outside this CS project
    private static readonly string NamespaceKey = "visitz_oauth_";
    private static readonly string AccessTokenKey = NamespaceKey + "access_token";
    private static readonly string RefreshTokenKey = NamespaceKey + "refresh_token";
    private static readonly string IdentityTokenKey = NamespaceKey + "identity_token";

    public static JwtSecurityToken AccessToken { get; private set; }

    public static JwtSecurityToken RefreshToken { get; private set; }

    public static JwtSecurityToken IdentityToken { get; private set; }

    private static async Task SetAsync(string key, string value)
    {
        await SecureStorage.Default.SetAsync(key, value);
    }

    private static async Task<string> GetAsync(string key)
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

    private static async Task SaveIdentityToken(string identityToken)
    {
        await SetAsync(IdentityTokenKey, identityToken);
        IdentityToken = new JwtSecurityToken(identityToken);
    }

    public static async Task SaveAsync(LoginResult loginResult)
    {
        await SaveAccessToken(loginResult.AccessToken);
        await SaveRefreshToken(loginResult.RefreshToken);
        await SaveIdentityToken(loginResult.IdentityToken);
    }

    public static async Task SaveAsync(RefreshTokenResult refreshResult)
    {
        await SaveAccessToken(refreshResult.AccessToken);
        await SaveRefreshToken(refreshResult.RefreshToken);
        await SaveIdentityToken(refreshResult.IdentityToken);
    }

    public static async Task<string> GetAccessTokenStringAsync()
    {
        return await GetAsync(AccessTokenKey);
    }

    public static async Task<string> GetRefreshTokenStringAsync()
    {
        return await GetAsync(RefreshTokenKey);
    }

    public static async Task<string> GetIdentityTokenStringAsync()
    {
        return await GetAsync(IdentityTokenKey);
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
        AccessToken = null;
    }

    public static void DeleteRefreshToken()
    {
        SecureStorage.Default.Remove(RefreshTokenKey);
        RefreshToken = null;
    }

    public static void DeleteIdentityToken()
    {
        SecureStorage.Default.Remove(IdentityTokenKey);
        RefreshToken = null;
    }

    private static bool IsTokenValid(JwtSecurityToken token)
    {
        return DateTime.UtcNow < token?.ValidTo;
    }

    public static async Task<bool> IsAccessTokenValid()
    {
        return IsTokenValid(await GetAccessTokenAsync());
    }

    public static async Task<bool> IsRefreshTokenExpired()
    {
        return !IsTokenValid(await GetRefreshTokenAsync());
    }
}
