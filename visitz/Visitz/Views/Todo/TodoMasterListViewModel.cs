using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Realms;
using Visitz.Messaging;
using Visitz.Resources.Localization;
using Visitz.Storage;
using Visitz.Views.BaseClasses;
using VisitzModel.Models.InPersonVisits;
using VisitzModel.Models.Navigation;

namespace Visitz.Views.Todo;

public partial class TodoMasterListViewModel : VisitzViewModel
{
    private bool _disposed;

    [ObservableProperty]
    public TodoItemUi selectedItem;
    TodoItemUi upcomingVisitsItem;

    [ObservableProperty]
    ObservableCollection<TodoItemUi> todoMasterItems = [];

    [ObservableProperty]
    public bool showEmpty = true;

    Realm icmDataRealm;

    protected override async Task InitAsync()
    {
        await base.InitAsync();
        icmDataRealm = await VisitzRealms.GetIcmDataRealmAsync();
        IQueryable<PersonVisit> query = PersonVisit.GetAllByType(icmDataRealm);

        upcomingVisitsItem = new TodoItemUi(
            LocalizedStrings.ChildYouthVisits,
            query,
            icmDataRealm,
            TodoItem_PropertyChanged,
            () => PersonVisit.GetUpcomingVisits(icmDataRealm).Count(),
            new NavItem() { ContentViewType = typeof(TodoVisitsView) }
        );
    }

    private void TodoItem_PropertyChanged(TodoItemUi item)
    {
        if (item.Count <= 0)
            TodoMasterItems.Remove(item);
        else if (!TodoMasterItems.Contains(item))
            InsertSortedAsc(TodoMasterItems, item);

        ShowEmpty = TodoMasterItems.Count <= 0;
    }

    static void InsertSortedAsc(ObservableCollection<TodoItemUi> collection, TodoItemUi todoItem)
    {
        if (collection.Count == 0)
            collection.Add(todoItem);
        else
        {
            var find = collection
                .OfType<TodoItemUi>()
                .FirstOrDefault(obj => obj.ItemName.CompareTo(todoItem.ItemName) >= 0);
            if (find != null)
                collection.Insert(collection.IndexOf(find), todoItem);
            else
                collection.Add(todoItem);
        }
    }

    [RelayCommand]
    public void MasterTodoItemSelected()
    {
        var msg = new TodoMasterSelectedMessage(upcomingVisitsItem.selectedTodoNavItem);
        StrongReferenceMessenger.Default.Send(msg);
    }

    protected override void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            upcomingVisitsItem?.Dispose();
            icmDataRealm?.Dispose();
            _disposed = true;
        }
        base.Dispose(disposing);
    }
}
