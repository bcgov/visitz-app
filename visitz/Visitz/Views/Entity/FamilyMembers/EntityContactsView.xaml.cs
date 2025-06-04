using Visitz.Views.BaseClasses;
using VisitzModel.Interfaces;
using VisitzModel.Models.Caseload;

namespace Visitz.Views.Entity.FamilyMembers;

public partial class EntityContactsView : ViewModelContentView, IBusinessObjectHolder
{
    public IBusinessObject BusinessObject
    {
        get => (ViewModel as IBusinessObjectHolder).BusinessObject;
        set => (ViewModel as IBusinessObjectHolder).BusinessObject = value;
    }

    public EntityContactsView() : base(ServiceProvider.GetService<EntityContactsViewModel>())
    {
        InitializeComponent();
        BindingContext = ViewModel;
    }
}
