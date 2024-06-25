namespace Visitz.Views.Navigation;

public partial class VerticalNavRailView : ViewModelContentView
{
	public VerticalNavRailView() : base(ServiceProvider.GetService<NavRailViewModel>())
	{
		InitializeComponent();
        BindingContext = ViewModel;
	}
}
