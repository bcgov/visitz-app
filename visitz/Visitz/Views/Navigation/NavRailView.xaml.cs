using Visitz.Pages;

namespace Visitz.Views.Navigation;

public partial class NavRailView : ViewModelContentView
{
	public NavRailView() : base(ServiceProvider.GetService<NavRailViewModel>())
	{
		InitializeComponent();
        BindingContext = ViewModel;
	}
}