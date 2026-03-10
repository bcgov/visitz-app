using Visitz.Views.BaseClasses;

namespace Visitz.Views.Root;

public partial class NavDrawerContentView : ViewModelContentView
{
    public NavDrawerContentView() : base(ServiceProvider.GetService<NavDrawerContentViewModel>())
    {
        InitializeComponent();
        BindingContext = ViewModel;
    }
}
