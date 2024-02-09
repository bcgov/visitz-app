using CommunityToolkit.Mvvm.ComponentModel;
using VisitzModel.Models;

namespace Visitz.ViewModels.Entity;

public partial class EntityContactsViewModel : VisitzViewModel, ICaseloadItemHolder
{
    [ObservableProperty]
    public CaseloadItem caseloadItem;

    [ObservableProperty]
    public IEnumerable<FamilyMember> contacts;

    public override void Create()
    {
        base.Create();

        Contacts = CaseloadItem.FamilyMembers;
    }
}
