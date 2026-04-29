using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Maui.Controls.Foldable;
using Visitz.Views.BaseClasses;
using Visitz.Views.Navigation;
using VisitzModel.Messaging;

namespace Visitz.Views.Root;

#nullable enable

public partial class RootViewModel : VisitzViewModel, IRecipient<AppNavMessage>
{
    [ObservableProperty]
    public bool isLandscape = false;

    [ObservableProperty]
    public bool isPortrait = false;

    [ObservableProperty]
    public bool showActivity = true;

    [ObservableProperty]
    public double minScreenSize = GetMinSize();

    public NavRailViewModel NavRailViewModel = ServiceProvider.GetService<NavRailViewModel>();

    static double GetMinSize()
    {
        if (DeviceInfo.Idiom == DeviceIdiom.Desktop)
            return 700; // 700 arbitrarily chosen
        else
        {
            double min = Math.Min(DeviceDisplay.MainDisplayInfo.Height, DeviceDisplay.MainDisplayInfo.Width);

            return min / DeviceDisplay.MainDisplayInfo.Density;
        }
    }

    protected override async Task InitAsync()
    {
        await base.InitAsync();

        UpdateOrientationVisibility(DeviceDisplay.Current.MainDisplayInfo.Orientation);

        DeviceDisplay.Current.MainDisplayInfoChanged += Current_MainDisplayInfoChanged;

        StrongReferenceMessenger.Default.RegisterAll(this);
    }

    bool disposed;

    protected override void Dispose(bool disposing)
    {
        if (!disposed && disposing)
        {
            StrongReferenceMessenger.Default.UnregisterAll(this);
            DeviceDisplay.Current.MainDisplayInfoChanged -= Current_MainDisplayInfoChanged;
            disposed = true;
        }

        base.Dispose(disposing);
    }

    private void Current_MainDisplayInfoChanged(object? sender, DisplayInfoChangedEventArgs e)
    {
        UpdateOrientationVisibility(e.DisplayInfo.Orientation);
    }

    private void UpdateOrientationVisibility(DisplayOrientation orientation)
    {
        IsPortrait = orientation.Equals(DisplayOrientation.Portrait);
        IsLandscape = !IsPortrait;
    }

    public void UpdateOrientationVisibility(TwoPaneViewMode mode)
    {
        IsPortrait = mode == TwoPaneViewMode.Tall;
        IsLandscape = mode == TwoPaneViewMode.Wide;
    }

    public void Receive(AppNavMessage message)
    {
        ShowActivity = false;
        StrongReferenceMessenger.Default.UnregisterAll(this);
    }
}
