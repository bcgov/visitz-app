using VisitzModel.Models.Caseload;
using VisitzModel.Models.EntityTypes;

namespace Visitz.Services;

internal class RecordServiceInfo(
    EntityType type,
    EntitySubtype subtype,
    string id,
    string fileNumber,
    string firstName,
    string lastName)
{
    public EntityType Type { get; set; } = type;

    public EntitySubtype Subtype { get; set; } = subtype;

    public string Id { get; set; } = id;

    public string FileNumber { get; set; } = fileNumber;

    public string FirstName { get; set; } = firstName;

    public string LastName { get; set; } = lastName;

    public RecordServiceInfo(IBusinessObject record) : this(
            record.EntityType,
            record.EntitySubtype,
            record.Id,
            record.FileNumber,
            record.GivenNames,
            record.LastName)
    { }
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
