using System.Collections.ObjectModel;
using System.ComponentModel;
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
    public TodoItemUi selectedItem;
    TodoItemUi upcomingVisitsItem;

    [ObservableProperty]
    ObservableCollection<object> todoMasterItems = [];

    [ObservableProperty]
    public bool showEmpty = true;

    Realm icmDataRealm;

    protected override async Task InitAsync()
    {
        await base.InitAsync();
        icmDataRealm = await VisitzRealms.GetIcmDataRealmAsync();
        IQueryable<PersonVisit> query = PersonVisit.GetUpcomingVisits(icmDataRealm);

        upcomingVisitsItem = new TodoItemUi(query, icmDataRealm);
        upcomingVisitsItem.PropertyChanged += TodoItem_PropertyChanged;
    }

    private void TodoItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(TodoItemUi.Count))
            return;

        TodoItemUi item = sender as TodoItemUi;

        if (item.Count <= 0)
            TodoMasterItems.Remove(item);
        else if (!TodoMasterItems.Contains(item))
            InsertSortedAsc(TodoMasterItems, item);

        ShowEmpty = TodoMasterItems.Count <= 0;
    }

    static void InsertSortedAsc(ObservableCollection<object> collection, TodoItemUi todoItem)
    {
        if (collection.Count == 0)
            collection.Add(todoItem);
        else
        {
            var find = collection.OfType<TodoItemUi>().FirstOrDefault(obj => obj.ItemName.CompareTo(todoItem.ItemName) >= 0);
            if (find != null)
                collection.Insert(collection.IndexOf(find), todoItem);
            else
                collection.Add(todoItem);
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            upcomingVisitsItem.PropertyChanged -= TodoItem_PropertyChanged;
            upcomingVisitsItem?.Dispose();
            icmDataRealm?.Dispose();
            _disposed = true;
        }
        base.Dispose(disposing);
    }
}
