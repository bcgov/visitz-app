// Adapted from https://stackoverflow.com/a/967098
namespace VisitzModel.Extensions;

public static class IListTExtensions
{
    /// <summary>
    /// <para>Searches the entire sorted IList for an element using the specified comparer and returns the zero-based index of the element.</para>
    /// <para>Implemented here as an extension because there is no built-in support for IList.</para>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static int BinarySearch<T>(
        this IList<T> list,
        T value,
        IComparer<T>? comparer = null,
        bool ascendingOrder = true,
        int startIndex = 0,
        int length = -1
    )
    {
        if (list == null)
            throw new ArgumentException(null, nameof(list));

        comparer ??= Comparer<T>.Default;

        int lower = startIndex;
        int upper = (length > 0 ? lower + length : list.Count) - 1;

        while (lower <= upper)
        {
            int mid = lower + (upper - lower) / 2;

            // TODO: replace this ternary with something more performant
            int comparisonResult = ascendingOrder
                ? comparer.Compare(value, list[mid])
                : comparer.Compare(list[mid], value);

            if (comparisonResult == 0)
                return mid;
            else if (comparisonResult < 0)
                upper = mid - 1;
            else
                lower = mid + 1;
        }

        return ~lower;
    }

    public static void InsertSorted<T>(
        this IList<T> list,
        T newItem,
        bool ascending = true,
        int startIndex = 0,
        int length = -1
    )
        where T : IComparable<T>
    {
        InsertSorted(list, newItem, Comparer<T>.Default, ascending, startIndex, length);
    }

    public static void InsertSorted<T>(
        this IList<T> list,
        T newItem,
        IComparer<T> comparer,
        bool ascending = true,
        int startIndex = 0,
        int length = -1
    )
    {
        if (list.Count == 0)
            list.Add(newItem);
        else
        {
            int index = list.BinarySearch(newItem, comparer, ascending, startIndex, length);

            if (index < 0)
                index = ~index;

            list.Insert(index, newItem);
        }
    }

    public static void AddAll<T>(this IList<T> list, IEnumerable<T> itemsToAdd)
    {
        foreach (var item in itemsToAdd)
            list.Add(item);
    }
}
