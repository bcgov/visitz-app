namespace Visitz.Behaviors;

public class StayUprightBehavior : Behavior<VisualElement>
{
    VisualElement? VisualElement { get; set; }

    protected override void OnAttachedTo(VisualElement bindable)
    {
        base.OnAttachedTo(bindable);

        VisualElement = bindable;
        BindingContext = bindable.BindingContext;

        DeviceDisplay.Current.MainDisplayInfoChanged += Current_MainDisplayInfoChanged;
        ApplyDeviceRotation(DeviceDisplay.Current.MainDisplayInfo.Rotation);
    }

    protected override void OnDetachingFrom(VisualElement bindable)
    {
        base.OnDetachingFrom(bindable);

        DeviceDisplay.Current.MainDisplayInfoChanged -= Current_MainDisplayInfoChanged;
        VisualElement = null;
    }

    void Current_MainDisplayInfoChanged(object? sender, DisplayInfoChangedEventArgs e)
    {
        ApplyDeviceRotation(e.DisplayInfo.Rotation);
    }

    void ApplyDeviceRotation(DisplayRotation rotation)
    {
        if (VisualElement == null)
            return;

        VisualElement.Rotation = rotation switch
        {
            DisplayRotation.Rotation90 => 90d,
            DisplayRotation.Rotation180 => 180d,
            DisplayRotation.Rotation270 => -90d,
            _ => 0d,
        };
    }
}
