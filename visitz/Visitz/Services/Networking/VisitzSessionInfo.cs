using System.IdentityModel.Tokens.Jwt;

namespace Visitz.Services.Networking
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
            var didGet = AccessToken.Payload.TryGetValue(key, out var tryOutput);
            output = (T)tryOutput;
            return didGet;
        }

        public string Idir => TryGet<string>(IdirUsernameKey, out var idir) ? idir : "";

        public string DisplayName => TryGet<string>(DisplayNameKey, out var displayName) ? displayName : "";
    }
}
