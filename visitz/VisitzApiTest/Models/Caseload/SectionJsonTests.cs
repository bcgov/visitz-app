using System.Text.Json;
using System.Text.Json.Nodes;
using VisitzApi.Json;
using VisitzApi.Models;
using VisitzApi.Models.Caseload;

namespace VisitzApiTest.Models.Caseload;

public class SectionJsonTests
{
    const string successCode = "200";
    const string sectionJsonSuccess =
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
    const string sectionJsonError =
$@"{{
    ""assignedIds"": [],
    ""status"": {errorCode},
    ""message"": {{
        ""message"": ""An error happened!""
    }}
}}";

    static string Interpolate(string jsonTemplate, string assignedId, string jsonObject)
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

        List<string> ids = (List<string>)section.GetType()
            .GetProperty(nameof(SectionJson<BaseRecordJson>.AssignedIds))!
            .GetValue(section)!;

        Assert.Contains(CaseJsonTests.assignedToId, ids);
    }

    [Theory]
    [InlineData(typeof(SectionJson<CaseJson>), CaseJsonTests.assignedToId, CaseJsonTests.json)]
    public void ParsesStatus(Type sectionType, string assignedId, string itemJson)
    {
        string json = Interpolate(sectionJsonSuccess, assignedId, itemJson);
        object section = JsonSerializer.Deserialize(json, sectionType, PayloadOptions.SiebelGet)!;

        int statusCode = (int)section.GetType()
            .GetProperty(nameof(SectionJson<BaseRecordJson>.Status))!
            .GetValue(section)!;

        Assert.Equal(int.Parse(successCode), statusCode);
    }

    [Theory]
    [InlineData(typeof(SectionJson<CaseJson>), CaseJsonTests.assignedToId, CaseJsonTests.json)]
    public void ParsesFirstItem(Type sectionType, string assignedId, string itemJson)
    {
        string json = Interpolate(sectionJsonSuccess, assignedId, itemJson);
        object section = JsonSerializer.Deserialize(json, sectionType, PayloadOptions.SiebelGet)!;

        var items = (IEnumerable<BaseRecordJson>)section.GetType()
            .GetProperty(nameof(SectionJson<BaseRecordJson>.Items))!
            .GetValue(section)!;

        Assert.NotNull(items.FirstOrDefault());
    }

    [Theory]
    [InlineData(typeof(SectionJson<CaseJson>))]
    public void ParsesMessage(Type sectionType)
    {
        object section = JsonSerializer.Deserialize(sectionJsonError, sectionType, PayloadOptions.SiebelGet)!;

        JsonObject message = (JsonObject)section.GetType()
            .GetProperty(nameof(SectionJson<BaseRecordJson>.Message))!
            .GetValue(section)!;

        Assert.NotNull(message);
    }

    [Theory]
    [InlineData(typeof(SectionJson<CaseJson>), CaseJsonTests.assignedToId, CaseJsonTests.json)]
    public void AssignedIdMatchesFirstItemAssignedToIdField(Type sectionType, string assignedId, string itemJson)
    {
        string json = Interpolate(sectionJsonSuccess, assignedId, itemJson);
        object section = JsonSerializer.Deserialize(json, sectionType, PayloadOptions.SiebelGet)!;

        var assignedIds = (List<string>)section.GetType()
            .GetProperty(nameof(SectionJson<BaseRecordJson>.AssignedIds))!
            .GetValue(section)!;

        var items = (IEnumerable<BaseRecordJson>)section.GetType()
            .GetProperty(nameof(SectionJson<BaseRecordJson>.Items))!
            .GetValue(section)!;

        string firstId = assignedIds.First();
        BaseRecordJson firstRecord = items.First();

        Assert.Equal(firstId, firstRecord.AssignedToId);
    }
}
