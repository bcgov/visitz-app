using Visitz.Views.BaseClasses;

namespace Visitz.Views.Entity.ChildYouthVisits;

public partial class ChildYouthVisitListView : ViewModelContentView
{
	public ChildYouthVisitListView() : base(ServiceProvider.GetService<ChildYouthVisitListViewModel>())
	{
		InitializeComponent();
        BindingContext = ViewModel;
	}
}
