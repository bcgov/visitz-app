using Visitz.Views.BaseClasses;
using VisitzModel.Models;

namespace Visitz.Views.Entity.ChildYouthVisits;

public partial class ChildYouthVisitListView : ViewModelContentView, ICaseloadItemHolder
{
	public CaseloadItem CaseloadItem
	{
		get => (ViewModel as ChildYouthVisitListViewModel).CaseloadItem;
		set => (ViewModel as ChildYouthVisitListViewModel).CaseloadItem = value;
	}

	public ChildYouthVisitListView() : base(ServiceProvider.GetService<ChildYouthVisitListViewModel>())
	{
		InitializeComponent();
		BindingContext = ViewModel;
	}
}
