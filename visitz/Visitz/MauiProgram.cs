using CommunityToolkit.Maui;
using Oidc;
using Syncfusion.Maui.Toolkit.Hosting;
using Visitz.Controls;
using Visitz.Controls.Handlers;
using Visitz.Settings;
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
            .UseMauiCommunityToolkitCamera()
            .ConfigureSyncfusionToolkit()
            .ConfigureEssentials(essentials =>
            {
                essentials.UseVersionTracking();
            })
#if IOS
            .ConfigureMauiHandlers(handlers =>
            {
                handlers.AddHandler<SelectableLabel, SelectableLabelHandler>();
            })
#endif
            .ConfigureVisitzLocalization()
            .ConfigureVisitzFonts()
            // TODO: Get AppSettings working correctly with DI
            .ConfigureOidcSettings(new AppSettings().Oidc)
            .ConfigureVisitzApi()
            .ConfigureVisitzLogging()
            .ConfigureVisitzScreens()
            .ConfigureVisitzApiServices()
            .ConfigureVisitzUtilities();

        VisitzDebugOptions.ConfigureVisitzDebugOptions();

#if IOS
        IOSHandlers.RegisterHandlers();
#endif

        return builder.Build();
    }
}
