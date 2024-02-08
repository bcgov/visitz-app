using CommunityToolkit.Mvvm.ComponentModel;
using Visitz.Models;
using Visitz.ViewModels;
using VisitzModel.Models;

namespace Visitz.Views.Entity;

public partial class EntityContactsViewModel : VisitzViewModel, ICaseloadItemHolder
{
    [ObservableProperty]
    public CaseloadItem caseloadItem;

    [ObservableProperty]
    public IEnumerable<FamilyMember> contacts;

    public override void PageCreated()
    {
        base.PageCreated();

        Contacts = CaseloadItem.FamilyMembers;
    }
}
