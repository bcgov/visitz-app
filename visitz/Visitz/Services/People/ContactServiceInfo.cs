using VisitzModel.Models.EntityTypes;

namespace Visitz.Services.People;

internal class ContactServiceInfo
{
    public EntityType Type { get; set; }

    public string Id { get; set; }

    public string Label { get; set; }
}
