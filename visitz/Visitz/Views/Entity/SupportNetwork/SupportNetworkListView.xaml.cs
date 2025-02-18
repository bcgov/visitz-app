using Visitz.Views.BaseClasses;

namespace Visitz.Views.Entity.SupportNetwork;

public partial class SupportNetworkListView : ViewModelContentView
{
	public SupportNetworkListView() : base(ServiceProvider.GetService<SupportNetworkListViewModel>())
	{
		InitializeComponent();
        BindingContext = ViewModel;
	}
}
