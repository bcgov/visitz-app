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
    public readonly ObservableCollection<TodoVisitsDisplayItem> todoItems = [];

    Realm icmDataRealm;

    protected override async Task InitAsync()
    {
        await base.InitAsync();

        icmDataRealm = await VisitzRealms.GetIcmDataRealmAsync();
        GetTodoItems(PersonVisit.GetUpcomingVisits(icmDataRealm));
    }

    public void GetTodoItems(IOrderedEnumerable<PersonVisit> items)
    {
        foreach (var item in items)
        {
            var caseloadItem = GetRelatedCaseloadItem(item);
            TodoItems.Add(new TodoVisitsDisplayItem(item, caseloadItem));
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
            icmDataRealm.Dispose();
            _disposed = true;
        }
        base.Dispose(disposing);
    }
}
