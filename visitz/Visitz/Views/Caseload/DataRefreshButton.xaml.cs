using Visitz.ViewModels;

namespace Visitz.Views.Caseload;

public partial class DataRefreshButton : ViewModelContentView
{
	public DataRefreshButton() : base(ServiceProvider.GetService<DataRefreshViewModel>())
	{
		InitializeComponent();
		BindingContext = ViewModel;
	}
}
