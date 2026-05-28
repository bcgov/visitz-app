using VisitzModel.Extensions.EntityTypes;
using VisitzModel.Models.Caseload;
using VisitzModel.Models.EntityTypes;

namespace Visitz.Services;

#nullable enable

internal class RecordServiceInfo(
    EntityType type,
    EntitySubtype subtype,
    string id,
    string fileNumber,
    string firstName,
    string lastName
) : IEquatable<RecordServiceInfo>
{
    public EntityType Type { get; } = type;

    public EntitySubtype Subtype { get; } = subtype;

    public string Id { get; } = id;

    public string FileNumber { get; } = fileNumber;

    public string FirstName { get; } = firstName;

    public string LastName { get; } = lastName;

    public RecordServiceInfo(IBusinessObject record)
        : this(
            record.EntityType,
            record.EntitySubtype,
            record.Id,
            record.FileNumber,
            record.GivenNames,
            record.LastName
        ) { }

    public bool Equals(RecordServiceInfo? other)
    {
        return Type == other?.Type && Id == other?.Id;
    }

    public override bool Equals(object? obj)
    {
        return obj is RecordServiceInfo info ? Equals(info) : base.Equals(obj);
    }

    public override int GetHashCode()
    {
        return Type.GetHashCode() * Id.GetHashCode();
    }

    public override string ToString()
    {
        try
        {
            return $"{Subtype.GetDisplayInitials()} {Type} {FileNumber} {LastName}, {FirstName}";
        }
        catch
        {
            return $"{Type} {FileNumber} {LastName}, {FirstName}";
        }
    }
}
