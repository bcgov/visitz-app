using Visitz.ViewModels.Drafts;

namespace Visitz.Views.Drafts;

public partial class DraftsList : ViewModelContentView
{
	public DraftsList() : base(ServiceProvider.GetService<DraftsListViewModel>())
	{
		InitializeComponent();
		BindingContext = ViewModel;
	}
}
