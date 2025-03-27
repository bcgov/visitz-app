using System.Text.Json;
using VisitzApi.Json;

namespace VisitzApiTest.Json;

public class PascalWhitespaceNamingPolicyTests
{
    class FieldJson
    {
        public string PascalName { get; set; } = string.Empty;

        public string APascalName { get; set; } = string.Empty;

        public string ABC {  get; set; } = string.Empty;

        public string PascalInfix123Numbers { get; set; } = string.Empty;

        public string PascalInFIXInitialism {  get; set; } = string.Empty;

        public string Word {  get; set; } = string.Empty;
    }

    const string someString = "some String";

    const string pascalName = "Pascal Name";
    const string aPascalName = "A Pascal Name";
    const string abc = "ABC";
    const string pascalInfix123Numbers = "Pascal Infix 123 Numbers";
    const string pascalInFIXInitialism = "Pascal In FIX Initialism";
    const string word = "Word";

    static string MakeJson(string fieldName)
    {
        return @$"{{""{fieldName}"": ""{someString}""}}";
    }

    [Fact]
    public void ParsesPascalNameCorrectly()
    {
        string json = MakeJson(pascalName);
        FieldJson obj = JsonSerializer.Deserialize<FieldJson>(json, PayloadOptions.SiebelGet)!;
        Assert.Equal(someString, obj.PascalName);
    }

    [Fact]
    public void ParsesAPascalNameCorrectly()
    {
        string json = MakeJson(aPascalName);
        FieldJson obj = JsonSerializer.Deserialize<FieldJson>(json, PayloadOptions.SiebelGet)!;
        Assert.Equal(someString, obj.APascalName);
    }

    [Fact]
    public void DoesNotChangeInitialisms()
    {
        string json = MakeJson(abc);
        FieldJson obj = JsonSerializer.Deserialize<FieldJson>(json, PayloadOptions.SiebelGet)!;
        Assert.Equal(someString, obj.ABC);
    }

    [Fact]
    public void ParsesInfixNumbersCorrectly()
    {
        string json = MakeJson(pascalInfix123Numbers);
        FieldJson obj = JsonSerializer.Deserialize<FieldJson>(json, PayloadOptions.SiebelGet)!;
        Assert.Equal(someString, obj.PascalInfix123Numbers);
    }

    [Fact]
    public void DoesNotModifyInfixInitialisms()
    {
        string json = MakeJson(pascalInFIXInitialism);
        FieldJson obj = JsonSerializer.Deserialize<FieldJson>(json, PayloadOptions.SiebelGet)!;
        Assert.Equal(someString, obj.PascalInFIXInitialism);
    }

    [Fact]
    public void DoesNotChangeSinglePascalWord()
    {
        string json = MakeJson(word);
        FieldJson obj = JsonSerializer.Deserialize<FieldJson>(json, PayloadOptions.SiebelGet)!;
        Assert.Equal(someString, obj.Word);
    }
}
