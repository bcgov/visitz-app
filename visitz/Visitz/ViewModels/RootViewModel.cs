using CommunityToolkit.Mvvm.ComponentModel;

namespace Visitz.ViewModels;

internal partial class RootViewModel : VisitzViewModel
{
    [ObservableProperty]
    public bool isLandscape = false;

    [ObservableProperty]
    public bool isPortrait = false;

    public override void PageCreated()
    {
        base.PageCreated();

        UpdateOrientationVisibility(DeviceDisplay.Current.MainDisplayInfo.Orientation);

        DeviceDisplay.Current.MainDisplayInfoChanged += Current_MainDisplayInfoChanged;
    }

    public override void PageDestroyed()
    {
        DeviceDisplay.Current.MainDisplayInfoChanged -= Current_MainDisplayInfoChanged;

        base.PageDestroyed();
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
