using CommunityToolkit.Mvvm.ComponentModel;
using Visitz.Views.BaseClasses;
using VisitzModel.Interfaces;
using VisitzModel.Models;
using VisitzModel.Models.People;
using VisitzModel.Sorting;

namespace Visitz.Views.Entity.FamilyMembers;

public partial class EntityContactsViewModel : VisitzViewModel, ICaseloadItemHolder
{
    [ObservableProperty]
    public CaseloadItem caseloadItem;

    [ObservableProperty]
    public IEnumerable<FamilyMember> contacts;

    public override void Create()
    {
        base.Create();

        Contacts = CaseloadItem.FamilyMembers.Order(new FamilyMemberComparer());
    }
}
