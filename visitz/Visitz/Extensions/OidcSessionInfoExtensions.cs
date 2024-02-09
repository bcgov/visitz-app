using Oidc;
using Visitz.Auth;

namespace Visitz.Extensions;

public static class OidcSessionInfoExtensions
{
    public static bool HasBasicAccessRole(this OidcSessionInfo info)
    {
        return info.Roles.Contains(VisitzRoles.BasicAccess);
    }
}
