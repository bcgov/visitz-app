using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Realms;
using Visitz.Storage;
using Visitz.Views.BaseClasses;
using VisitzModel.Models.Drafts;
using VisitzModel.Models.InPersonVisits;

namespace Visitz.Views.Todo;

public partial class TodoMasterListViewModel : VisitzViewModel
{
    private bool _disposed;

    [ObservableProperty]
    public MasterDraftItem selectedItem;
    TodoItemUi todoItem;

    [ObservableProperty]
    ObservableCollection<object> todoMasterItems = [];

    [ObservableProperty]
    bool showEmpty;

    protected override async Task InitAsync()
    {
        await base.InitAsync();
        Realm icmDataRealm = await VisitzRealms.GetIcmDataRealmAsync();
        IQueryable<PersonVisit> query = PersonVisit.GetUpcomingVisits(icmDataRealm);

        todoItem = new TodoItemUi(query, icmDataRealm);
        TodoMasterItems.Add(todoItem);

        ShowEmpty = TodoMasterItems.Count < 1;
    }

    protected override void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            todoItem?.Dispose();
            _disposed = true;
        }
        base.Dispose(disposing);
    }
}
