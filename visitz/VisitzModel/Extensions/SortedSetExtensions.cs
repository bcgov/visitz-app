namespace VisitzModel.Extensions;

public static class SortedSetExtensions
{
    public static IList<T> AsList<T>(this SortedSet<T> set)
    {
        var list = new List<T>();

        foreach (var item in set)
            list.Add(item);

        return list;
    }
}
