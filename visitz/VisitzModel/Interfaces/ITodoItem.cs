using VisitzModel.Models.EntityTypes;

namespace VisitzModel.Interfaces;

public interface ITodoItem : IComparable<ITodoItem>
{
    object Item { get; }

    public int SortOrder { get; }

    EntityType RelatedEntityType { get; }

    EntitySubtype RelatedEntitySubtype { get; }
}
