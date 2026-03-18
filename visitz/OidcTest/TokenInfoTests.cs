using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;
using Oidc;

namespace OidcTest;

public class TokenInfoTests
{
    const string displayNameKey = "display_name";
    const string arbitraryDisplayName = "an arbitrary display name";

    const string stringCollectionClaimKey = "items";
    const string arbitraryItem = "this collection is full of these";

    const string issuer = "";
    const string audience = "";
    static readonly DateTime ArbitraryNotBefore = DateTime.Parse("2024-02-09 12:00:00");
    static readonly DateTime ArbitraryExpires = DateTime.Parse("2024-02-09 12:05:00");

    static TokenInfo CreateTokenInfo(DateTime notBefore, DateTime expires, IEnumerable<Claim> claims)
    {
        return new TokenInfo(
            new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                notBefore: notBefore,
                expires: expires
            )
        );
    }

    [Fact]
    public void CanCreateTokenWithStringClaim()
    {
        var tokenInfo = CreateTokenInfo(
            ArbitraryNotBefore,
            ArbitraryExpires,
            [new Claim(displayNameKey, arbitraryDisplayName)]
        );

        Assert.NotNull(tokenInfo);
    }

    [Fact]
    public void CanCreateAndReadTokenWithStringClaim()
    {
        var tokenInfo = CreateTokenInfo(
            ArbitraryNotBefore,
            ArbitraryExpires,
            [new Claim(displayNameKey, arbitraryDisplayName)]
        );

        var actualName = tokenInfo.TryGet<string>(displayNameKey, out var claim) ? claim : null;

        Assert.Equal(arbitraryDisplayName, actualName);
    }

    [Fact]
    public void CanCreateTokenAndReadListClaim()
    {
        var tokenInfo = CreateTokenInfo(
            ArbitraryNotBefore,
            ArbitraryExpires,
            [
                new Claim(displayNameKey, arbitraryDisplayName),
                new Claim(stringCollectionClaimKey, arbitraryItem),
                new Claim(stringCollectionClaimKey, arbitraryItem),
            ]
        );

        if (tokenInfo.TryGet<List<object>>(stringCollectionClaimKey, out var actualOutput))
            Assert.Equal(arbitraryItem, actualOutput.FirstOrDefault());
    }

    [Fact]
    public void CanCreateTokenAndReadJsonArrayClaim()
    {
        var jsonItems = JsonSerializer.Serialize(new List<string>() { arbitraryItem, arbitraryItem });
        var tokenInfo = CreateTokenInfo(
            ArbitraryNotBefore,
            ArbitraryExpires,
            [
                new Claim(displayNameKey, arbitraryDisplayName),
                new Claim(stringCollectionClaimKey, jsonItems, JsonClaimValueTypes.JsonArray),
            ]
        );

        if (tokenInfo.TryGet<JsonElement>(stringCollectionClaimKey, out var actualOutput))
            Assert.Equal(arbitraryItem, actualOutput.EnumerateArray().FirstOrDefault().ToString());
    }
}
