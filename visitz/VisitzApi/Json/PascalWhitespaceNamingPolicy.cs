using System.Text.Json;
using System.Text.RegularExpressions;

namespace VisitzApi.Json;

public partial class PascalWhitespaceNamingPolicy : JsonNamingPolicy
{
    const string LowerThenUpperNumber = "(?<1>[a-z])(?<2>[A-Z0-9])"; // camelCase -> camel Case
    const string NumberThenUpper = @"(?<1>[0-9])(?<2>[A-Z])";
    const string UpperThenNumber = @"(?<1>[A-Z])(?<2>[0-9])";
    const string UpperThenPascal = @"(?<1>[A-Z])(?<2>[A-Z](?=[a-z]))"; // ABCItem -> ABC Item

    const string PascalWhitespaceBoundary =
        LowerThenUpperNumber + "|" + NumberThenUpper + "|" + UpperThenNumber + "|" + UpperThenPascal;

    [GeneratedRegex(PascalWhitespaceBoundary)]
    internal static partial Regex PascalBoundaryRegex();

    public override string ConvertName(string name)
    {
        return PascalBoundaryRegex().Replace(name, "$1 $2");
    }
}
