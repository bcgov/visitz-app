using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml.Media.Imaging;
using Visitz.Services.Caseload;
using Visitz.Storage;
using Visitz.Views.Debugging;
using Grid = Microsoft.UI.Xaml.Controls.Grid;
using Image = Microsoft.UI.Xaml.Controls.Image;

namespace Visitz;

public partial class VisitzWindow
{
    const string HeightKey = $"VisitzApp.Height";
    const string WidthKey = $"VisitzApp.Width";

    readonly Grid _scrimGrid = new() { Visibility = Microsoft.UI.Xaml.Visibility.Collapsed };

    bool _autoRefreshTriedOnce;

    void SetupForWindows()
    {
        Height = Preferences.Get(HeightKey, InitialHeight);
        Width = Preferences.Get(WidthKey, Height * InitialWidthRatio);

        Destroying += VisitzWindow_Destroying;
        SetupScrimGrid();
    }

    void SetupScrimGrid()
    {
        var nativeWindow = Handler?.PlatformView as Microsoft.UI.Xaml.Window;
        if (nativeWindow?.Content is not Microsoft.UI.Xaml.Controls.Panel panel)
            return;

        Image scrimImage = new()
        {
            Stretch = Microsoft.UI.Xaml.Media.Stretch.UniformToFill,
            Source = new BitmapImage(new Uri($"ms-appx:///{BcGovAlbum.GetFeaturedPictureUri()}")),
        };

        _scrimGrid.Children.Add(scrimImage);
        panel.Children.Add(_scrimGrid);
    }

    partial void Platform_OnActivated()
    {
        // Run in OnActivated instead of OnResumed to respond to window focus events
        TryRunAutoRefresh();

        OnWindowFocusChanged(true);
    }

    partial void Platform_OnDeactivated()
    {
        OnWindowFocusChanged(false);
    }

    /// <summary>
    /// Runs the AutoRefreshService after discarding the first attempt. This
    /// is done as a workaround MAUI lifecycles—if we don't discard the first
    /// run the app will crash.
    /// </summary>
    void TryRunAutoRefresh()
    {
        if (_autoRefreshTriedOnce)
            WeakReferenceMessenger.Default.Send(AutoRefreshService.MakeStartMessage());
        else
            _autoRefreshTriedOnce = true;
    }

    void OnWindowFocusChanged(bool focused)
    {
        if (DebugOptions.DisablePrivacyScrim)
            return;

        _scrimGrid.Visibility = focused ? Microsoft.UI.Xaml.Visibility.Collapsed : Microsoft.UI.Xaml.Visibility.Visible;
    }

    void VisitzWindow_Destroying(object? sender, EventArgs e)
    {
        Preferences.Set(HeightKey, Height);
        Preferences.Set(WidthKey, Width);
        ServiceProvider.GetService<ILogger<VisitzWindow>>().LogInformation($"Saved window dims ({Width},{Height})");
    }
}
