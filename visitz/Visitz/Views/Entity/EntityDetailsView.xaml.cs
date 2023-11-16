using Visitz.Models;

namespace Visitz.Views.Entity;

public partial class EntityDetailsView : ViewModelContentView, ICaseloadItemHolder
{
	public CaseloadItem CaseloadItem
	{
		get => (ViewModel as ICaseloadItemHolder).CaseloadItem;
        set => (ViewModel as ICaseloadItemHolder).CaseloadItem = value;
    }
	public EntityDetailsView() : base(ServiceProvider.GetService<EntityDetailsViewModel>())
	{
		InitializeComponent();
		BindingContext = ViewModel;
	}
}