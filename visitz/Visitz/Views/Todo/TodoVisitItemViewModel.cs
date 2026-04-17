using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Visitz.Views.BaseClasses;
using Visitz.Views.Caseload;
using VisitzModel.Interfaces;
using VisitzModel.Messaging;
using VisitzModel.Models.Caseload;
using VisitzModel.Models.InPersonVisits;
using VisitzModel.Models.Navigation;

namespace Visitz.Views.Todo;

#nullable enable

internal partial class TodoVisitItemViewModel(PersonVisit visit) : VisitzViewModel, ITodoItem
{
    public int SortOrder => Visit.IsValid ? Visit.DueDateDaysRemaining : int.MinValue;

    public PersonVisit Visit { get; set; } = visit;

    public IBusinessObject BusinessObject { get; set; } = CaseRecord.GetByPersonVisitItem(visit.Realm, visit);

    public bool IsOverdue => Visit.IsValid && DateTimeOffset.Now.Date > Visit.DueDate;

    public int CompareTo(ITodoItem? other)
    {
        return other == null ? 1 : SortOrder.CompareTo(other.SortOrder);
    }

    [RelayCommand]
    private static void TodoItemSelected(TodoVisitItemViewModel item)
    {
        NavigateTo(item.BusinessObject, EntitySection.ChildYouthVisits);
    }

    static void NavigateTo(IBusinessObject businessObject, EntitySection section)
    {
        var appNav = new AppNavMessage(new() { ContentViewType = typeof(CaseloadContainerView) });
        StrongReferenceMessenger.Default.Send(appNav);

        var caseloadNav = new BusinessObjectSelectedMessage(businessObject, section);
        StrongReferenceMessenger.Default.Send(caseloadNav);
    }
}
