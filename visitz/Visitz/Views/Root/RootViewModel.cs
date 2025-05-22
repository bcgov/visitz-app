using CommunityToolkit.Mvvm.ComponentModel;
using Visitz.Views.BaseClasses;

namespace Visitz.Views.Root;

internal partial class RootViewModel : VisitzViewModel
{
    [ObservableProperty]
    public bool isLandscape = false;

    [ObservableProperty]
    public bool isPortrait = false;

    protected override Task InitAsync()
    {
        base.InitAsync();

        UpdateOrientationVisibility(DeviceDisplay.Current.MainDisplayInfo.Orientation);

        DeviceDisplay.Current.MainDisplayInfoChanged += Current_MainDisplayInfoChanged;

        return Task.CompletedTask;
    }

    bool disposed;
    protected override void Dispose(bool disposing)
    {
        if (!disposed && disposing)
        {
            DeviceDisplay.Current.MainDisplayInfoChanged -= Current_MainDisplayInfoChanged;
            disposed = true;
        }

        base.Dispose(disposing);
    }

    private void Current_MainDisplayInfoChanged(object sender, DisplayInfoChangedEventArgs e)
    {
        UpdateOrientationVisibility(e.DisplayInfo.Orientation);
    }

    private void UpdateOrientationVisibility(DisplayOrientation orientation)
    {
        IsPortrait = orientation.Equals(DisplayOrientation.Portrait);
        IsLandscape = !IsPortrait;
    }
}
