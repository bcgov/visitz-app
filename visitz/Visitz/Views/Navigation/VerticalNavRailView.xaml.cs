using CommunityToolkit.Maui.Behaviors;
using Visitz.Views.BaseClasses;
using Visitz.Views.Debugging;

namespace Visitz.Views.Navigation;

#nullable enable

public partial class VerticalNavRailView : ViewModelContentView<NavRailViewModel>
{
    public VerticalNavRailView()
        : base(ServiceProvider.GetService<NavRailViewModel>())
    {
        InitializeComponent();
        BindingContext = ViewModel;

        if (DebugOptions.Enabled)
            SetupDebugOptionsEntry();
    }

    void SetupDebugOptionsEntry()
    {
        var menu = new MenuFlyout();

        var item = new MenuFlyoutItem() { Text = "Debug options", Command = ViewModel.OpenDebugOptionsCommand };

        item.KeyboardAccelerators.Add(new KeyboardAccelerator() { Key = "F2" });

        menu.Add(item);

        FlyoutBase.SetContextFlyout(LogoImage, menu);

        LogoImage.Behaviors.Add(
            new TouchBehavior()
            {
                BindingContext = ViewModel,
                LongPressDuration = 300,
                LongPressCommand = ViewModel.OpenDebugOptionsCommand,
                PressedAnimationDuration = 300,
                PressedScale = 1.4d,
                PressedAnimationEasing = Easing.BounceIn,
            }
        );
    }
}
