using System.Text.Json;

namespace Oidc;

public class OidcSessionInfo
{
    private static readonly string IdirUsernameKey = "idir_username";
    private static readonly string DisplayNameKey = "display_name";
    private static readonly string RolesKey = "client_roles";
    private static readonly string PreferredUsernameKey = "preferred_username";
    private static readonly string GivenNameKey = "given_name";
    private static readonly string FamilyNameKey = "family_name";
    private static readonly string EmailKey = "email";
    private static readonly string OfficesKey = "offices";
    private static readonly string OfficesDelimiter = "<;:?:;>";

    private static OidcSessionInfo SessionInfo { get; set; }

    private TokenInfo Token { get; set; }

    public static async Task<OidcSessionInfo> GetAsync()
    {
        SessionInfo ??= new OidcSessionInfo();
        SessionInfo.Token = new TokenInfo(await TokenHolder.GetAccessTokenAsync());
        return SessionInfo;
    }

    public event EventHandler<HashSet<string>> OfficesChanged;

    private OidcSessionInfo() { }

    private string GetIdir()
    {
        return Token.TryGet<string>(IdirUsernameKey, out var idir) ? idir : "";
    }

    private List<string> GetRoles()
    {
        List<string> outRoles = [];

        if (Token.TryGet<JsonElement>(RolesKey, out var roles))
            foreach (var role in roles.EnumerateArray())
                outRoles.Add(role.ToString());

        return outRoles;
    }

    private static string GetInitialOrNull(string name)
    {
        return name?.Length > 0 ? name[0].ToString() : null;
    }

    public string Idir => GetIdir();

    public string DisplayName => Token.TryGet<string>(DisplayNameKey, out var displayName) ? displayName : "";

    public List<string> Roles => GetRoles();

    public string PreferredUsername => Token.TryGet<string>(PreferredUsernameKey, out var name) ? name : "";

    public string GivenName => Token.TryGet<string>(GivenNameKey, out var givenName) ? givenName : "";

    public string FamilyName => Token.TryGet<string>(FamilyNameKey, out var familyName) ? familyName : "";

    public string FirstLastName => $"{GivenName} {FamilyName}";

    public string Email => Token.TryGet<string>(EmailKey, out var email) ? email : "";

    public string UserInitials
    {
        get
        {
            var initials = GetInitialOrNull(GivenName) + GetInitialOrNull(FamilyName);
            return initials?.Length > 0 ? initials : "--";
        }
    }

    public HashSet<string> OfficeNames
    {
        get => new(Preferences.Default.Get(OfficesKey, "").Split(OfficesDelimiter));
        set
        {
            if (value.Count == 0)
            {
                Preferences.Default.Remove(OfficesKey);
                OfficesChanged?.Invoke(this, value);
            }
            else if (OfficeNames == null || !OfficeNames.Equals(value))
            {
                Preferences.Default.Set(
                    OfficesKey,
                    value.Aggregate((accum, officeName) => accum + OfficesDelimiter + officeName)
                );

                OfficesChanged?.Invoke(this, value);
            }
        }
    }
}
