using System.Text.Json;
using VisitzApi.Json;
using VisitzApi.Models.Caseload;

namespace VisitzApiTest.Models.Caseload;

public partial class CaseloadJsonTests
{
    const string caseloadJson =
        @$"{{
    ""cases"": {SectionJsonTests.sectionJsonSuccess},
    ""incidents"": {SectionJsonTests.sectionJsonSuccess},
    ""srs"":null,
    ""memos"":null
}}";

    [Theory]
    [InlineData(nameof(CaseloadJson.Cases), CaseJsonTests.assignedToId, CaseJsonTests.json)]
    public void FieldsNotNull(string propertyName, string assignedId, string itemJson)
    {
        string json = SectionJsonTests.Interpolate(caseloadJson, assignedId, itemJson);

        CaseloadJson caseload = JsonSerializer.Deserialize<CaseloadJson>(json, PayloadOptions.SiebelGet)!;
        object baseRecord = caseload.GetType().GetProperty(propertyName)!.GetValue(caseload)!;

        Assert.NotNull(baseRecord);
    }
}
