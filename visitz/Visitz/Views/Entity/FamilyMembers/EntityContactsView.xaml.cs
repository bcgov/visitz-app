using Visitz.Views.BaseClasses;
using VisitzModel.Interfaces;
using VisitzModel.Models.Caseload;

namespace Visitz.Views.Entity.FamilyMembers;

#nullable enable

public partial class EntityContactsView : ViewModelContentView, IBusinessObjectHolder
{
    new EntityContactsViewModel? ViewModel => base.ViewModel as EntityContactsViewModel;

    public IBusinessObject? BusinessObject
    {
        get => ViewModel?.BusinessObject;
        set
        {
            if (ViewModel != null)
                ViewModel.BusinessObject = value;
        }
    }

    public EntityContactsView()
        : base(ServiceProvider.GetService<EntityContactsViewModel>())
    {
        InitializeComponent();
        BindingContext = ViewModel;
    }
}
