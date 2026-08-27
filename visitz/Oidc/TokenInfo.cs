using System.IdentityModel.Tokens.Jwt;

namespace Oidc;

public class TokenInfo(JwtSecurityToken jwt)
{
    public JwtSecurityToken Jwt { get; } = jwt;

    public bool TryGet<T>(string key, out T output)
    {
        object tryOutput = null;
        var didGet = Jwt?.Payload?.TryGetValue(key, out tryOutput) ?? false;

        output = tryOutput != null ? (T)tryOutput : default;
        return didGet;
    }
}
