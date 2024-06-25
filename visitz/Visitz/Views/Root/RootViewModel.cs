using CommunityToolkit.Mvvm.ComponentModel;
using Visitz.Views.BaseClasses;

namespace Visitz.Views.Root;

internal partial class RootViewModel : VisitzViewModel
{
    [ObservableProperty]
    public bool isLandscape = false;

    [ObservableProperty]
    public bool isPortrait = false;

    public override void Create()
    {
        base.Create();

        UpdateOrientationVisibility(DeviceDisplay.Current.MainDisplayInfo.Orientation);

        DeviceDisplay.Current.MainDisplayInfoChanged += Current_MainDisplayInfoChanged;
    }

    public override void Destroy()
    {
        DeviceDisplay.Current.MainDisplayInfoChanged -= Current_MainDisplayInfoChanged;

        base.Destroy();
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
