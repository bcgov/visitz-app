using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Maui.Platform;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using System.ComponentModel;
using Visitz.Services.Caseload;
using Visitz.Storage;
using static PdfSharp.Capabilities.Features;
using Grid = Microsoft.UI.Xaml.Controls.Grid;
using Image = Microsoft.UI.Xaml.Controls.Image;
using Window = Microsoft.Maui.Controls.Window;


namespace Visitz;

public partial class VisitzWindow
{
    private static readonly double InitialHeight = 800;
    private static readonly double WidthRatio = 1.5d;
    static Microsoft.UI.Xaml.Controls.Grid ScrimGrid;
    static Image ScrimImage;
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
    partial void OnWindowFocusChanged(bool focused)
    {
        var nativeWindow = this.Handler?.PlatformView as Microsoft.UI.Xaml.Window;

        if (nativeWindow?.Content is not FrameworkElement root)
            return;

        if (ScrimGrid == null)
        {
            ScrimGrid = new Grid();

            ScrimImage = new Image
            {
                Stretch = Microsoft.UI.Xaml.Media.Stretch.UniformToFill,
                Source = new BitmapImage(new Uri("ms-appx:///bella_coola_river.jpg"))
            };

            ScrimGrid.Children.Add(ScrimImage);

            var panel = root as Panel;

            if (panel != null)
                panel.Children.Add(ScrimGrid);
        }

        ScrimGrid.Visibility = focused
            ? Microsoft.UI.Xaml.Visibility.Collapsed
            : Microsoft.UI.Xaml.Visibility.Visible;
        ScrimGrid.Opacity = focused ? 0 : 1;
    }


}
