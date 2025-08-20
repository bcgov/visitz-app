using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Visitz.Views.BaseClasses;
using VisitzModel.Messaging;

namespace Visitz.Views.Root;

internal partial class RootViewModel : VisitzViewModel, IRecipient<AppNavMessage>
{
    [ObservableProperty]
    public bool isLandscape = false;

    [ObservableProperty]
    public bool isPortrait = false;

    [ObservableProperty]
    public bool showActivity = true;

    protected override Task InitAsync()
    {
        base.InitAsync();

        UpdateOrientationVisibility(DeviceDisplay.Current.MainDisplayInfo.Orientation);

        DeviceDisplay.Current.MainDisplayInfoChanged += Current_MainDisplayInfoChanged;

        StrongReferenceMessenger.Default.RegisterAll(this);

        return Task.CompletedTask;
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

    private void Current_MainDisplayInfoChanged(object sender, DisplayInfoChangedEventArgs e)
    {
        UpdateOrientationVisibility(e.DisplayInfo.Orientation);
    }

    private void UpdateOrientationVisibility(DisplayOrientation orientation)
    {
        IsPortrait = orientation.Equals(DisplayOrientation.Portrait);
        IsLandscape = !IsPortrait;
    }

    public void Receive(AppNavMessage message)
    {
        ShowActivity = false;
        StrongReferenceMessenger.Default.UnregisterAll(this);
    }
}
