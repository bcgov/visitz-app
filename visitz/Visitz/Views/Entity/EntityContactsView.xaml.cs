using VisitzModel.Models;

namespace Visitz.Views.Entity;

public partial class EntityContactsView : ViewModelContentView, ICaseloadItemHolder
{
    public CaseloadItem CaseloadItem
    {
        get => (ViewModel as ICaseloadItemHolder).CaseloadItem;
        set => (ViewModel as ICaseloadItemHolder).CaseloadItem = value;
    }

    public EntityContactsView() : base(ServiceProvider.GetService<EntityContactsViewModel>())
    {
		InitializeComponent();
        BindingContext = ViewModel;
	}
}