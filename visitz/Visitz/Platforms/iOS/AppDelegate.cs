using Foundation;
using Visitz.Services.AppLogs;

namespace Visitz;

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
    private CrashFileStore? _crashFileStore;

    public AppDelegate()
    {
        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
    }

    protected override MauiApp CreateMauiApp()
    {
        MauiApp app = MauiProgram.CreateMauiApp();

        _crashFileStore = app.Services.GetRequiredService<CrashFileStore>();

        _crashFileStore.Initialize();

        return app;
    }

    private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        if (e.ExceptionObject is Exception exception)
        {
            _crashFileStore?.PersistSynchronously(exception);
        }
    }
}
