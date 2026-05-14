using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Visitz.FontIcons;
using Visitz.Resources.Localization;
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
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SortOrder))]
    [NotifyPropertyChangedFor(nameof(Visit))]
    public partial object Item { get; set; }

    public int SortOrder => Visit.DueDateDaysRemaining;

    public EntityType RelatedEntityType => BusinessObject?.EntityType ?? EntityType.Unknown;

    public EntitySubtype RelatedEntitySubtype => BusinessObject?.EntitySubtypeBinding ?? EntitySubtype.Unknown;

    public PersonVisit Visit => (PersonVisit)Item;

    public IBusinessObject? BusinessObject { get; set; }

    [ObservableProperty]
    public partial bool IsOverdue { get; set; }

    [ObservableProperty]
    public partial string Description { get; set; } = string.Empty;

    [ObservableProperty]
    public partial DateTimeOffset DueDate { get; set; }

    public string IconGlyph => MaterialIcons.Person_pin_circle;

    public TodoVisitItemViewModel(PersonVisit visit)
    {
        ArgumentNullException.ThrowIfNull(visit.Realm);

        Item = visit;
        BusinessObject = CaseRecord.GetByPersonVisitItem(visit.Realm, visit);

        ApplyVisit(visit);
        visit.PropertyChanged += Visit_PropertyChanged;
    }

    bool _disposed;

    protected override void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            Visit.PropertyChanged -= Visit_PropertyChanged;
            _disposed = true;
        }
        base.Dispose(disposing);
    }

    private void Visit_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (sender != null)
            ApplyVisit((PersonVisit)sender);
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

    void ApplyVisit(PersonVisit visit)
    {
        IsOverdue = DateTimeOffset.Now.Date > visit.DueDate;
        Description = IsOverdue ? LocalizedStrings.OverdueVisit : LocalizedStrings.UpcomingVisit;
        DueDate = visit.DueDate;
    }
}
