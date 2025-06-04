using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Realms;
using Visitz.Storage;
using Visitz.Views.BaseClasses;
using Visitz.Views.Caseload;
using VisitzModel.Messaging;
using VisitzModel.Models;
using VisitzModel.Models.InPersonVisits;
using VisitzModel.Models.Navigation;

namespace Visitz.Views.Todo;

public partial class TodoVisitsViewModel : VisitzViewModel
{
    private bool _disposed;

    [ObservableProperty]
    public ObservableCollection<TodoVisitsDisplayItem> todoItems = [];

    Realm icmDataRealm;

    readonly ObservableRealmQueryMap realmQuery = new();

    protected override async Task InitAsync()
    {
        await base.InitAsync();

        icmDataRealm = await VisitzRealms.GetIcmDataRealmAsync();
        realmQuery.ItemsChanged += RealmQuery_ItemsChanged;
        realmQuery.Subscribe(icmDataRealm, PersonVisit.GetAllByType(icmDataRealm));
    }

    private void RealmQuery_ItemsChanged(object sender, (Type Type, IRealmCollection<IRealmObject> Items, ChangeSet Changes) e)
    {
        if (e.Type == typeof(PersonVisit))
            UpdateTodoItemsList(e.Items, e.Changes);
    }

    private void UpdateTodoItemsList(IRealmCollection<IRealmObject> items, ChangeSet changes)
    {
        var upcomingVisits = PersonVisit.GetUpcomingVisits(icmDataRealm).ToList();
        TodoItems.Clear();

        foreach (var visit in upcomingVisits)
        {
            var caseloadItem = GetRelatedCaseloadItem(visit);
            TodoItems.Add(new TodoVisitsDisplayItem(visit, caseloadItem));
        }
    }

    [RelayCommand]
    private static void TodoItemSelected(TodoVisitsDisplayItem item)
    {
        if (item.CaseloadItem != null)
            NavigateTo(item.CaseloadItem, item.SectionToOpen, item.TodoItem);
    }

    private CaseloadItem GetRelatedCaseloadItem(PersonVisit todoItem)
    {
        var caseloadItem = icmDataRealm
            .All<CaseloadItem>()
            .Where(item => item.RowId == todoItem.ParentId)
            .FirstOrDefault();
        return caseloadItem;
    }

    static void NavigateTo(CaseloadItem caseloadItem, EntitySection section, PersonVisit visitItem)
    {
        var caseloadNav = new CaseloadItemSelectedMessage(caseloadItem, section);
        StrongReferenceMessenger.Default.Send(caseloadNav);

        var appNav = new AppNavMessage(new() { ContentViewType = typeof(CaseloadContainerView) });
        StrongReferenceMessenger.Default.Send(appNav);
    }

    protected override void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            realmQuery.ItemsChanged -= RealmQuery_ItemsChanged;
            realmQuery.Dispose();
            icmDataRealm.Dispose();
            _disposed = true;
        }
        base.Dispose(disposing);
    }
}
