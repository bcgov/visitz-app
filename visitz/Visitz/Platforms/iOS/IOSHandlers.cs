#if IOS

namespace Visitz.Platforms.iOS;

public class IOSHandlers
{
    public static void RegisterHandlers()
    {
        RemoveInputAccessoryViewFromEditor();
    }

    /// <summary>
    /// <para>MAUI adds a "done" bar above the soft keyboard, reducing screen availability for "dismiss" functionality
    /// that is already present on the soft keyboard. This mapping removes that bar.</para>
    /// 
    /// <para>iPhone's soft keyboard apparently does not have the OS "dismiss" functionality, so we may need to revisit
    /// this in the future.</para>
    /// 
    /// https://github.com/dotnet/maui/issues/17768#issuecomment-1744244568
    /// </summary>
    private static void RemoveInputAccessoryViewFromEditor()
    {
        Microsoft.Maui.Handlers.EditorHandler.Mapper.AppendToMapping("EditorChange", (handler, view) =>
        {
            handler.PlatformView.InputAccessoryView = null;
        });
    }
}

#endif
