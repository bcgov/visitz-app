using Visitz.Views.BaseClasses;

namespace Visitz.Views.Navigation;

#nullable enable

public partial class HorizontalNavRailView : ViewModelContentView<NavRailViewModel>
{
    public HorizontalNavRailView()
        : base(ServiceProvider.GetService<NavRailViewModel>())
    {
        InitializeComponent();
        BindingContext = ViewModel;
    }
}
