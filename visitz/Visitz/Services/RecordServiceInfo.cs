using VisitzModel.Models.EntityTypes;

namespace Visitz.Services;

internal class RecordServiceInfo
{
    public EntityType Type { get; set; }

    public string Id { get; set; }

    public string Label { get; set; }

    RecordServiceInfo(string id, string label)
    {
        Id = id;
        Label = label;
    }

    public RecordServiceInfo(CaseRecord @case) : this(@case.Id, @case.Name)
    {
        Type = EntityType.Case;
    }

    public RecordServiceInfo(IncidentRecord incident) : this(incident.Id, incident.Name)
    {
        Type = EntityType.Incident;
    }
}
