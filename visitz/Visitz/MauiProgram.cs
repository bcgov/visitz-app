using CommunityToolkit.Maui;
using Visitz.VisitzConfig;

#if IOS
using Visitz.Platforms.iOS;
#endif

namespace Visitz;

/// <summary>
/// The program that gets invoked before anything else by the .NET runtime.
/// </summary>
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        return CreateVisitzApp();
    }

    private static MauiApp CreateVisitzApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<VisitzApp>()
            .UseMauiCommunityToolkit()
            .ConfigureEssentials(essentials =>
            {
                essentials.UseVersionTracking();
            })
            .ConfigureVisitzLocalization()
            .ConfigureVisitzFonts()
            .ConfigureVisitzAuth()
            .ConfigureVisitzApi()
            .ConfigureVisitzLogging()
            .ConfigureVisitzScreens()
            .ConfigureVisitzServices();

        VisitzDebugOptions.ConfigureVisitzDebugOptions();

#if IOS
        IOSHandlers.RegisterHandlers();
#endif

        return builder.Build();
    }
}

