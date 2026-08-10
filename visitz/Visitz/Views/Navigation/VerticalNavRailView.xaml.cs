using CommunityToolkit.Maui.Behaviors;
using CommunityToolkit.Mvvm.Input;
using Visitz.Views.BaseClasses;
using Visitz.Views.Debugging;

namespace Visitz.Views.Navigation;

public partial class VerticalNavRailView : ViewModelContentView<NavRailViewModel>
{
    public VerticalNavRailView()
        : base(ServiceProvider.GetService<NavRailViewModel>())
    {
        InitializeComponent();
        BindingContext = ViewModel;

        if (DebugOptions.Default.Enabled)
            SetupDebugOptionsUsage();
    }

    void SetupDebugOptionsUsage()
    {
        var menu = new MenuFlyout();
        FlyoutBase.SetContextFlyout(LogoImage, menu);

        void AddHotkey(
            string key,
            string title,
            IRelayCommand command,
            KeyboardAcceleratorModifiers modifiers = KeyboardAcceleratorModifiers.None
        )
        {
            MenuFlyoutItem item = new() { Text = title, Command = command };
            item.KeyboardAccelerators.Add(new KeyboardAccelerator() { Key = key, Modifiers = modifiers });
            menu.Add(item);
        }

        AddHotkey("F9", "Swap window width and height", ViewModel.SwapWindowDimensionsCommand);
        AddHotkey(
            "F9",
            "Apply phone dimensions",
            ViewModel.ApplyPhoneDimensionsCommand,
            KeyboardAcceleratorModifiers.Ctrl
        );
        AddHotkey(
            "F9",
            "Apply tablet dimensions",
            ViewModel.ApplyTabletDimensionsCommand,
            KeyboardAcceleratorModifiers.Shift
        );
        AddHotkey(
            "F9",
            "Apply default desktop dimensions",
            ViewModel.ApplyDefaultDesktopDimensionsCommand,
            KeyboardAcceleratorModifiers.Windows
        );
        AddHotkey("F12", "Open debug options", ViewModel.OpenDebugOptionsCommand);

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
