using CommunityToolkit.Maui;
using Visitz.Platforms.iOS;
using Visitz.VisitzConfig;

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

