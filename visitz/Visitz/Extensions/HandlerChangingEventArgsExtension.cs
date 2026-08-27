namespace Visitz.Extensions;

public static class HandlerChangingEventArgsExtension
{
    public static bool AttachingToHandler(this HandlerChangingEventArgs args)
    {
        return args.NewHandler != null && args.OldHandler == null;
    }

    public static bool DetachingFromHandler(this HandlerChangingEventArgs args)
    {
        return args.NewHandler == null && args.OldHandler != null;
    }
}
