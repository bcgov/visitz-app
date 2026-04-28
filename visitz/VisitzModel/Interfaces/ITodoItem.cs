namespace VisitzModel.Interfaces;

public interface ITodoItem : IComparable<ITodoItem>
{
    public int SortOrder { get; }
}
