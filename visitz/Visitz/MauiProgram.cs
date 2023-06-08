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
            .ConfigureVisitzLocalization()
            .ConfigureVisitzFonts()
            .ConfigureVisitzAuth()
            .ConfigureVisitzApi()
            .ConfigureVisitzLogging()
            .ConfigureVisitzScreens();

        VisitzDebugOptions.ConfigureVisitzDebugOptions();

        return builder.Build();
    }
}

