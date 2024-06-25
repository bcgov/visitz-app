using Visitz.Views.BaseClasses;

namespace Visitz.Views.Drafts;

public partial class DraftsMasterList : ViewModelContentView
{
	public DraftsMasterList() : base(ServiceProvider.GetService<DraftsMasterListViewModel>())
	{
		InitializeComponent();
		BindingContext = ViewModel;
	}
}
