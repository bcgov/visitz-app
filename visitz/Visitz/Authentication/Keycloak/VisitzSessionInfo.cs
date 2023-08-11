using System.Collections;
using System.IdentityModel.Tokens.Jwt;
using Visitz.Storage;

namespace Visitz.Authentication.Keycloak
{
    public class VisitzSessionInfo
    {
        private static readonly string IdirUsernameKey = "idir_username";
        private static readonly string DisplayNameKey = "display_name";
        private static readonly string RolesKey = "client_roles";
        private static readonly string PreferredUsernameKey = "preferred_username";
        private static readonly string GivenNameKey = "given_name";
        private static readonly string FamilyNameKey = "family_name";
        private static readonly string EmailKey = "email";

        private static VisitzSessionInfo SessionInfo { get; set; }

        private JwtSecurityToken AccessToken { get; set; }

        public static async Task<VisitzSessionInfo> GetAsync()
        {
            SessionInfo ??= new VisitzSessionInfo();
            SessionInfo.AccessToken = await TokenHolder.GetAccessTokenAsync();
            return SessionInfo;
        }

        private VisitzSessionInfo() { }

        private bool TryGet<T>(string key, out T output)
        {
            object tryOutput = null;
            var didGet = AccessToken?.Payload?.TryGetValue(key, out tryOutput) ?? false;

            output = (T)tryOutput;
            return didGet;
        }

        private string GetIdir()
        {
            return DebugOptions.IdirOverride.Length > 0 
                ? DebugOptions.IdirOverride 
                : TryGet<string>(IdirUsernameKey, out var idir) ? idir : "";
        }

        private List<string> GetRoles()
        {
            List<string> outRoles = new();

            if (TryGet<IEnumerable>(RolesKey, out var roles))
                foreach (var role in roles)
                    outRoles.Add(role.ToString());

            return outRoles;
        }

        public string Idir => GetIdir();

        public string DisplayName => TryGet<string>(DisplayNameKey, out var displayName) ? displayName : "";

        public List<string> Roles => GetRoles();

        public string PreferredUsername => TryGet<string>(PreferredUsernameKey, out var name) ? name : "";

        public string GivenName => TryGet<string>(GivenNameKey, out var givenName) ? givenName : "";

        public string FamilyName => TryGet<string>(FamilyNameKey, out var familyName) ? familyName : "";

        public string Email => TryGet<string>(EmailKey, out var email) ? email : "";
    }
}
