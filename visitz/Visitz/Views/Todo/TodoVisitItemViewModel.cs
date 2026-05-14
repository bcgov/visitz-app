using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Visitz.Views.BaseClasses;
using VisitzModel.Interfaces;
using VisitzModel.Messaging;
using VisitzModel.Models.Caseload;
using VisitzModel.Models.EntityTypes;
using VisitzModel.Models.InPersonVisits;
using VisitzModel.Models.Navigation;

namespace Visitz.Views.Todo;

#nullable enable

internal partial class TodoVisitItemViewModel : VisitzViewModel, ITodoItem
{
    public object Item { get; set; }

    public int SortOrder => Visit.DueDateDaysRemaining;

    public EntityType RelatedEntityType => BusinessObject?.EntityType ?? EntityType.Unknown;

    public EntitySubtype RelatedEntitySubtype => BusinessObject?.EntitySubtypeBinding ?? EntitySubtype.Unknown;

    public PersonVisit Visit => (PersonVisit)Item;

    public IBusinessObject? BusinessObject { get; set; }

    public bool IsOverdue => Visit.IsValid && DateTimeOffset.Now.Date > Visit.DueDate;

    public TodoVisitItemViewModel(PersonVisit visit)
    {
        Item = visit;

        ArgumentNullException.ThrowIfNull(visit.Realm);

        BusinessObject = CaseRecord.GetByPersonVisitItem(visit.Realm, visit);
    }

    public int CompareTo(ITodoItem? other)
    {
        return other == null ? 1 : SortOrder.CompareTo(other.SortOrder);
    }

    [RelayCommand]
    private static void TodoItemSelected(TodoVisitItemViewModel item)
    {
        if (item.BusinessObject != null)
            NavigateTo(item.BusinessObject, EntitySection.ChildYouthVisits);
    }

    static void NavigateTo(IBusinessObject businessObject, EntitySection section)
    {
        var caseloadNav = new BusinessObjectSelectedMessage(businessObject, section);
        StrongReferenceMessenger.Default.Send(caseloadNav);
    }
}
