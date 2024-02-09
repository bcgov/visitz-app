using Visitz.ViewModels;

namespace Visitz.Views.Navigation;

public partial class HorizontalNavRailView : ViewModelContentView
{
	public HorizontalNavRailView() : base(ServiceProvider.GetService<NavRailViewModel>())
	{
		InitializeComponent();
		BindingContext = ViewModel;
	}
}