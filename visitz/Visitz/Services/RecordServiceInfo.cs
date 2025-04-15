using VisitzModel.Models.Caseload;
using VisitzModel.Models.EntityTypes;

namespace Visitz.Services;

internal class RecordServiceInfo(
    EntityType type,
    string id,
    string fileNumber,
    string firstName,
    string lastName)
{
    public EntityType Type { get; set; } = type;

    public string Id { get; set; } = id;

    public string FileNumber { get; set; } = fileNumber;

    public string FirstName { get; set; } = firstName;

    public string LastName { get; set; } = lastName;

    RecordServiceInfo(
        string id, EntityType type, IBusinessObject record) : this(
            type,
            id,
            record.FileNumber,
            record.GivenNames,
            record.LastName)
    { }

    public RecordServiceInfo(CaseRecord @case) : this(@case.Id, @case.EntityType, @case) { }

    public RecordServiceInfo(IncidentRecord incident) : this(incident.Id, incident.EntityType, incident) { }

    public RecordServiceInfo(MemoRecord memo) : this(memo.Id, memo.EntityType, memo) { }

    public RecordServiceInfo(ServiceRequestRecord sr) : this(sr.Id, sr.EntityType, sr) { }
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
