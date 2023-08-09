using System.IdentityModel.Tokens.Jwt;
using Visitz.Storage;

namespace Visitz.Authentication.Keycloak
{
    public class VisitzSessionInfo
    {
        private static readonly string IdirUsernameKey = "idir_username";
        private static readonly string DisplayNameKey = "display_name";

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

        public string Idir => GetIdir();

        public string DisplayName => TryGet<string>(DisplayNameKey, out var displayName) ? displayName : "";
    }
}
