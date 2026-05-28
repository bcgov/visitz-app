namespace Visitz.Services;

#nullable enable

internal class ApiRangeItemException<T>(T item, Exception exception) : Exception(exception.Message, exception)
{
    public T Item { get; private set; } = item;
}
