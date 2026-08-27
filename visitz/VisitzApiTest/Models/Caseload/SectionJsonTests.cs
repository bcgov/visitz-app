using System.Text.Json;
using VisitzApi.Json;
using VisitzApi.Models.Base;
using VisitzApi.Models.Caseload;

namespace VisitzApiTest.Models.Caseload;

public class SectionJsonTests
{
    const string empty = "";
    const string successCode = "200";
    internal const string sectionJsonSuccess =
        $@"{{
    ""assignedIds"": [
        ""//assignedId//""
    ],
    ""status"": {successCode},
    ""items"": [
        //item//
    ]
}}";

    const string errorCode = "500";
    const string aMessage = "Here's a message!";
    const string errorMessage = "a more detailed error";

    const string sectionJsonNestedMessage =
        $@"{{
    ""assignedIds"": [],
    ""status"": {errorCode},
    ""message"": {{
        ""message"": ""{aMessage}""
    }}
}}";

    const string sectionJsonNestedErrorWithMessage =
        $@"{{
    ""assignedIds"": [],
    ""status"": {errorCode},
    ""message"": {{
        ""message"": ""{aMessage}"",
        ""somekey"": {{
            ""ERROR"": ""{errorMessage}""
        }}
    }}
}}";

    const string sectionJsonShallowError =
        $@"{{
    ""assignedIds"": [],
    ""status"": {errorCode},
    ""message"": {{
        ""ERROR"": ""{errorMessage}""
    }}
}}";

    internal static string Interpolate(string jsonTemplate, string assignedId, string jsonObject)
    {
        string json = jsonTemplate.Replace("//assignedId//", assignedId);
        return json.Replace("//item//", jsonObject);
    }

    [Theory]
    [InlineData(typeof(SectionJson<CaseJson>), CaseJsonTests.assignedToId, CaseJsonTests.json)]
    public void ParsesAssignedIds(Type sectionType, string assignedId, string itemJson)
    {
        string json = Interpolate(sectionJsonSuccess, assignedId, itemJson);
        object section = JsonSerializer.Deserialize(json, sectionType, PayloadOptions.SiebelGet)!;

        List<string> ids =
            (List<string>)
                section
                    .GetType()
                    .GetProperty(nameof(SectionJson<AssignableRecordJson>.AssignedIds))!
                    .GetValue(section)!;

        Assert.Contains(CaseJsonTests.assignedToId, ids);
    }

    [Theory]
    [InlineData(typeof(SectionJson<CaseJson>), CaseJsonTests.assignedToId, CaseJsonTests.json)]
    public void ParsesStatus(Type sectionType, string assignedId, string itemJson)
    {
        string json = Interpolate(sectionJsonSuccess, assignedId, itemJson);
        object section = JsonSerializer.Deserialize(json, sectionType, PayloadOptions.SiebelGet)!;

        int statusCode = (int)
            section.GetType().GetProperty(nameof(SectionJson<AssignableRecordJson>.Status))!.GetValue(section)!;

        Assert.Equal(int.Parse(successCode), statusCode);
    }

    [Theory]
    [InlineData(typeof(SectionJson<CaseJson>), CaseJsonTests.assignedToId, CaseJsonTests.json)]
    public void ParsesFirstItem(Type sectionType, string assignedId, string itemJson)
    {
        string json = Interpolate(sectionJsonSuccess, assignedId, itemJson);
        object section = JsonSerializer.Deserialize(json, sectionType, PayloadOptions.SiebelGet)!;

        var items =
            (IEnumerable<AssignableRecordJson>)
                section.GetType().GetProperty(nameof(SectionJson<AssignableRecordJson>.Items))!.GetValue(section)!;

        Assert.NotNull(items.FirstOrDefault());
    }

    [Theory]
    [InlineData(aMessage, sectionJsonNestedMessage)]
    [InlineData(aMessage, sectionJsonNestedErrorWithMessage)]
    [InlineData(empty, sectionJsonShallowError)]
    public void ParsesFirstMessage(string expected, string json)
    {
        var section = JsonSerializer.Deserialize<SectionJson<AssignableRecordJson>>(json, PayloadOptions.SiebelGet)!;

        Assert.Equal(expected, section.GetFirstMessage());
    }

    [Theory]
    [InlineData(errorMessage, sectionJsonShallowError)]
    [InlineData(errorMessage, sectionJsonNestedErrorWithMessage)]
    [InlineData(empty, sectionJsonNestedMessage)]
    public void ParsesFirstError(string expected, string json)
    {
        var section = JsonSerializer.Deserialize<SectionJson<AssignableRecordJson>>(json, PayloadOptions.SiebelGet)!;

        Assert.Equal(expected, section.GetFirstError());
    }

    [Theory]
    [InlineData(typeof(SectionJson<CaseJson>), CaseJsonTests.assignedToId, CaseJsonTests.json)]
    public void AssignedIdMatchesFirstItemAssignedToIdField(Type sectionType, string assignedId, string itemJson)
    {
        string json = Interpolate(sectionJsonSuccess, assignedId, itemJson);
        object section = JsonSerializer.Deserialize(json, sectionType, PayloadOptions.SiebelGet)!;

        var assignedIds =
            (List<string>)
                section
                    .GetType()
                    .GetProperty(nameof(SectionJson<AssignableRecordJson>.AssignedIds))!
                    .GetValue(section)!;

        var items =
            (IEnumerable<AssignableRecordJson>)
                section.GetType().GetProperty(nameof(SectionJson<AssignableRecordJson>.Items))!.GetValue(section)!;

        string firstId = assignedIds.First();
        AssignableRecordJson firstRecord = items.First();

        Assert.Equal(firstId, firstRecord.AssignedToId);
    }
}
