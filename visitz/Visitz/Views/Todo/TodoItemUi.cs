using CommunityToolkit.Mvvm.ComponentModel;
using Realms;
using Visitz.Views.BaseClasses;
using VisitzModel.Models;
using VisitzModel.Models.Navigation;

namespace Visitz.Views.Todo;

public partial class TodoItemUi : VisitzViewModel
{
    readonly ObservableRealmQueryMap realmQuery = new();

    readonly List<IRealmObject> todoItems = [];

    [ObservableProperty]
    int count;

    [ObservableProperty]
    string itemName;

    readonly Func<int> counter;

    readonly Action<TodoItemUi> action;

    public NavItem selectedTodoNavItem;

    public TodoItemUi(
        string name,
        IQueryable<IRealmObject> query,
        Realm icmDataRealm,
        Action<TodoItemUi> countUpdated,
        Func<int> getCount = null,
        NavItem navItem = null
    )
    {
        ItemName = name;
        realmQuery.ItemsChanged += RealmQuery_ItemsChanged;
        action = countUpdated;
        counter = getCount;
        selectedTodoNavItem = navItem;
        realmQuery.Subscribe(icmDataRealm, query);
    }

    private void RealmQuery_ItemsChanged(
        object sender,
        (Type Type, IRealmCollection<IRealmObject> Items, ChangeSet Changes) e
    )
    {
        if (counter == null)
        {
            UpdateTodoItemsList(e.Items, e.Changes);
            Count = todoItems.Count;
        }
        else
            Count = counter();

        action(this);
    }

    private void UpdateTodoItemsList(IRealmCollection<IRealmObject> items, ChangeSet changes)
    {
        if (changes == null)
        {
            foreach (var item in items)
                todoItems.Add(item);
        }
        else
        {
            foreach (int deleted in changes.DeletedIndices.Reverse())
                todoItems.RemoveAt(deleted);

            foreach (int inserted in changes.InsertedIndices)
                todoItems.Insert(inserted, items[inserted]);
        }
    }
}
