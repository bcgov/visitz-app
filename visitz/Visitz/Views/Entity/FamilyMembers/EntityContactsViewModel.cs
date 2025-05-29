using CommunityToolkit.Mvvm.ComponentModel;
using Visitz.Views.BaseClasses;
using VisitzModel.Interfaces;
using VisitzModel.Models.Caseload;
using VisitzModel.Models.People;

namespace Visitz.Views.Entity.FamilyMembers;

public partial class EntityContactsViewModel : VisitzViewModel, IBusinessObjectHolder
{
    [ObservableProperty]
    public IBusinessObject businessObject;

    [ObservableProperty]
    public IEnumerable<IcmContact> contacts;

    protected override Task InitAsync()
    {
        var init = base.InitAsync();

        Contacts = BusinessObject.Contacts.ToList().Order(new IcmContactRelationshipComparer());

        return init;
    }
}
