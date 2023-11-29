// Adapted from https://stackoverflow.com/a/967098

namespace Visitz.Extensions;

public static class IListTExtensions
{
    public static int BinarySearch<T>(this IList<T> list, T value, IComparer<T> comparer = null)
    {
        if (list == null)
            throw new ArgumentException(null, nameof(list));

        comparer ??= Comparer<T>.Default;

        int lower = 0;
        int upper = list.Count - 1;

        while (lower <= upper)
        {
            int mid = lower + (upper - lower) / 2;
            int comparisonResult = comparer.Compare(value, list[mid]);

            if (comparisonResult == 0)
                return mid;
            else if (comparisonResult < 0)
                upper = mid - 1;
            else
                lower = mid + 1;
        }

        return ~lower;
    }
}
