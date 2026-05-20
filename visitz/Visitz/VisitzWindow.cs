using Microsoft.Extensions.Logging;
using Visitz.Views.AppLock;
using Visitz.Views.User;

namespace Visitz;

public partial class VisitzWindow : Window
{
    // Arbitrarily chosen dimensions
    public static readonly double InitialHeight = 800;
    public static readonly double InitialWidthRatio = 1.5d;

    public VisitzWindow() { }

    public VisitzWindow(Page page)
        : base(page) { }

    protected override async void OnCreated()
    {
        base.OnCreated();

#if WINDOWS
        SetupForWindows();
#endif

        await SessionPage.TryOpenAsync(animated: false);
        await AppLockPage.TryPrompt(promptOnAppearing: true);

        LogScreen();
    }

    protected override async void OnStopped()
    {
        base.OnStopped();

        await AppLockPage.TryPrompt(promptOnAppearing: false);
    }

    partial void Platform_OnActivated();

    protected override async void OnActivated()
    {
        base.OnActivated();

        await SessionPage.TryOpenAsync(animated: false);

        Platform_OnActivated();
    }

    partial void Platform_OnDeactivated();

    protected override void OnDeactivated()
    {
        base.OnDeactivated();

        Platform_OnDeactivated();
    }

    void LogScreen()
    {
        string deviceDims = $"{DeviceDisplay.MainDisplayInfo.Width}w,{DeviceDisplay.MainDisplayInfo.Height}";
        string windowDims = $"{Width}w,{Height}h";
        string dims = $"Device dimensions: {deviceDims} // Window dimensions: {windowDims}";

        ServiceProvider.GetService<ILogger<VisitzWindow>>().LogInformation(dims);
    }
}
