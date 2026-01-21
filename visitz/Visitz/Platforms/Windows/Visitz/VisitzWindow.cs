using CommunityToolkit.Mvvm.Messaging;
using Visitz.Services.Caseload;

namespace Visitz;

public partial class VisitzWindow
{
    private static readonly double InitialHeight = 800;
    private static readonly double WidthRatio = 1.5d;

    bool AutoRefreshTriedOnce { get; set; }

    private static partial Window ApplyDefaultWindowLayout(Window window)
    {
        window.Height = InitialHeight;
        window.Width = window.Height * WidthRatio;

        return window;
    }

    /// <summary>
    /// Runs the AutoRefreshService after discarding the first attempt. This
    /// is done as a workaround MAUI lifecycles—if we don't discard the first
    /// run the app will crash.
    /// </summary>
    partial void TryRunAutoRefresh()
    {
        if (AutoRefreshTriedOnce)
            WeakReferenceMessenger.Default.Send(AutoRefreshService.MakeStartMessage());
        else
            AutoRefreshTriedOnce = true;
    }
}
