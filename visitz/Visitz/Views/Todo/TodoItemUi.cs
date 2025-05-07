using CommunityToolkit.Mvvm.ComponentModel;
using Realms;
using Visitz.Resources.Localization;
using Visitz.Views.BaseClasses;
using VisitzModel.Models;

namespace Visitz.Views.Todo;

public partial class TodoItemUi : VisitzViewModel
{
    readonly ObservableRealmQueryMap realmQuery = new();

    List<IRealmObject> todoItems = [];

    [ObservableProperty]
    int count;

    [ObservableProperty]
    string itemName;

    public TodoItemUi(IQueryable<IRealmObject> query, Realm icmDataRealm)
    {
        realmQuery.ItemsChanged += RealmQuery_ItemsChanged;
        realmQuery.Subscribe(icmDataRealm, query);
    }
    private void RealmQuery_ItemsChanged(object sender, (Type Type, IRealmCollection<IRealmObject> Items, ChangeSet Changes) e)
    {
        UpdateTodoItemsList(e.Items, e.Changes);
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
        Count = todoItems.Count;
        ItemName = LocalizedStrings.ChildYouthVisits;
    }
}
