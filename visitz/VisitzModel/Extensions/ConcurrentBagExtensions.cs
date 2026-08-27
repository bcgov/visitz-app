using System.Collections.Concurrent;

namespace VisitzModel.Extensions;

public static class ConcurrentBagExtensions
{
    public static void AddAll<T>(this ConcurrentBag<T> bag, IEnumerable<T> items)
    {
        foreach (var item in items)
            bag.Add(item);
    }
}
