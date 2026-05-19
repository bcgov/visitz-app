using System.Collections;

namespace VisitzModel.Extensions;

public static class IListTExtensions
{
    extension<T>(IList<T> list)
    {
        // Adapted from https://stackoverflow.com/a/967098
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
        public int BinarySearch(
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

        public void InsertSorted(
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

        public void AddAll(IEnumerable<T> itemsToAdd)
        {
            if (itemsToAdd is ICollection collection)
                ArrayList.Adapter((IList)list).AddRange(collection);
            else
            {
                foreach (var item in itemsToAdd)
                    list.Add(item);
            }
        }

        public void Sort(IComparer<T> comparer)
        {
            ArrayList.Adapter((IList)list).Sort((IComparer?)comparer);
        }
    }

    extension<T>(IList<T> list)
        where T : IComparable<T>
    {
        public void InsertSorted(T newItem, bool ascending = true, int startIndex = 0, int length = -1)
        {
            InsertSorted(list, newItem, Comparer<T>.Default, ascending, startIndex, length);
        }

        public void Sort(bool ascending = true)
        {
            Comparison<T> comparison = ascending ? (a, b) => a.CompareTo(b) : (a, b) => a.CompareTo(b) * -1;

            Sort(list, Comparer<T>.Create(comparison));
        }
    }
}
