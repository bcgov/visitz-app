using Visitz.Views.BaseClasses;
using VisitzModel.Interfaces;
using VisitzModel.Models;

namespace Visitz.Views.Entity.FamilyMembers;

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
