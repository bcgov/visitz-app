namespace Visitz.Views.Entity;

#nullable enable

internal interface IAsyncInitialize
{
    Task? InitTask { get; }
}
