using Visitz.VisitzConfig;
using CommunityToolkit.Maui;

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

        return builder.Build();
    }
}

