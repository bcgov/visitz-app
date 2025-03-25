using VisitzModel.Models.Caseload;
using VisitzModel.Models.EntityTypes;

namespace Visitz.Services;

internal class RecordServiceInfo
{
    public EntityType Type { get; set; }

    public string Id { get; set; }

    public string Label { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    RecordServiceInfo(string id, string label, string firstName = null, string lastName = null)
    {
        Id = id;
        Label = label;
        FirstName = firstName;
        LastName = lastName;
    }

    public RecordServiceInfo(CaseRecord @case) : this(@case.Id, @case.Name, @case.GivenNames, @case.LastName)
    {
        Type = EntityType.Case;
    }

    public RecordServiceInfo(IncidentRecord incident) : this(incident.Id, incident.LastName, incident.GivenNames, incident.LastName)
    {
        Type = EntityType.Incident;
    }

    public RecordServiceInfo(MemoRecord memo) : this(memo.Id, memo.LastName)
    {
        Type = EntityType.Memo;
    }

    public RecordServiceInfo(ServiceRequestRecord sr) : this(sr.Id, sr.LastName)
    {
        Type = EntityType.ServiceRequest;
    }
}

internal static class RecordServiceInfoExtensions
{
    public static Exception CombineIntoException(this List<ApiRangeItemException<RecordServiceInfo>> list)
    {
        var outString = list.Select(ex =>
        {
            return $"• {ex.Item.Type} {ex.Item.Label} -> {ex.Message}";
        }).Aggregate((accum, item) => accum + Environment.NewLine + item);

        return new Exception(outString);
    }
}
