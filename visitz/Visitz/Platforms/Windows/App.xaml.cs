// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

using Microsoft.Extensions.Logging;
using VisitzModel.Platforms.Windows.Logging;

namespace Visitz.WinUI;

/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public partial class App : MauiWinUIApplication
{
    /// <summary>
    /// Initializes the singleton application object.  This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App()
    {
        this.InitializeComponent();
        UnhandledException += App_UnhandledException;
    }

    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

    private void App_UnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
    {
        EventLogWriter.WriteEntry(LogLevel.Error, e.Exception.Message, GetType().FullName, exception: e.Exception);
    }
}
