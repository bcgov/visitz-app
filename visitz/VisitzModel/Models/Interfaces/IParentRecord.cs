using VisitzModel.Models.EntityTypes;

namespace VisitzModel.Models.Interfaces;

public interface IParentRecord
{
    public string ParentId { get; set; }

    public EntityType ParentType { get; set; }
}
