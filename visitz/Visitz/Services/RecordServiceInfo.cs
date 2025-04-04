using VisitzModel.Models.Caseload;
using VisitzModel.Models.EntityTypes;

namespace Visitz.Services;

internal class RecordServiceInfo
{
    public EntityType Type { get; set; }

    public string Id { get; set; }

    public string FileNumber { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    RecordServiceInfo(string id, string fileNumber, string firstName, string lastName)
    {
        Id = id;
        FileNumber = fileNumber;
        FirstName = firstName;
        LastName = lastName;
    }

    RecordServiceInfo(
        string id, IBusinessObject record) : this(id, record.FileNumber, record.GivenNames, record.LastName) { }

    public RecordServiceInfo(CaseRecord @case) : this(@case.Id, @case)
    {
        Type = EntityType.Case;
    }

    public RecordServiceInfo(IncidentRecord incident) : this(incident.Id, incident)
    {
        Type = EntityType.Incident;
    }

    public RecordServiceInfo(MemoRecord memo) : this(memo.Id, memo)
    {
        Type = EntityType.Memo;
    }

    public RecordServiceInfo(ServiceRequestRecord sr) : this(sr.Id, sr)
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
            return $"• {ex.Item.Type} {ex.Item.FileNumber} -> {ex.Message}";
        }).Aggregate((accum, item) => accum + Environment.NewLine + item);

        return new Exception(outString);
    }
}
