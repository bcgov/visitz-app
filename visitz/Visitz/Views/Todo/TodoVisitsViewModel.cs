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
        if (changes == null)
        {
            foreach (var item in items)
            {
                var personVisit = item as PersonVisit;
                var caseloadItem = GetRelatedCaseloadItem(personVisit);
                TodoItems.Add(new TodoVisitsDisplayItem(personVisit, caseloadItem));
            }
        }
        else
        {
            foreach (int deleted in changes.DeletedIndices.Reverse())
                TodoItems.RemoveAt(deleted);

            foreach (int inserted in changes.InsertedIndices)
            {
                var personVisit = items[inserted] as PersonVisit;
                var caseloadItem = GetRelatedCaseloadItem(personVisit);
                TodoItems.Insert(inserted, new TodoVisitsDisplayItem(personVisit, caseloadItem));
            }
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
