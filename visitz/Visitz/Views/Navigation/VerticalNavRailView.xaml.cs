using Visitz.Views.BaseClasses;
using Visitz.Views.Debugging;

namespace Visitz.Views.Navigation;

public partial class VerticalNavRailView : ViewModelContentView
{
    public VerticalNavRailView() : base(ServiceProvider.GetService<NavRailViewModel>())
    {
        InitializeComponent();
        BindingContext = ViewModel;

        if (DebugOptions.Enabled)
            SetupDebugOptionsEntry();
    }

    void SetupDebugOptionsEntry()
    {
        var menu = new MenuFlyout();

        NavRailViewModel vm = (NavRailViewModel)ViewModel;

        var item = new MenuFlyoutItem()
        {
            Text = "Debug options",
            Command = vm.OpenDebugOptionsCommand,
        };

        item.KeyboardAccelerators.Add(new KeyboardAccelerator()
        {
            Key = "F2"
        });

        menu.Add(item);

        FlyoutBase.SetContextFlyout(LogoImage, menu);
    }
}
