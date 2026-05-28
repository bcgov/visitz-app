namespace Visitz.Services;

#nullable enable

internal class ApiRangeItemException<T>(T item, Exception exception)
    : Exception(item?.ToString() + " -> " + exception.Message, exception)
{
    public T Item { get; private set; } = item;
}
