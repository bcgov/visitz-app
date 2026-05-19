using System.Collections.ObjectModel;
using System.Collections.Specialized;
using CommunityToolkit.Mvvm.ComponentModel;
using Realms;
using Visitz.Storage;
using Visitz.Views.BaseClasses;
using VisitzModel.Extensions;
using VisitzModel.Interfaces;
using VisitzModel.Models;
using VisitzModel.Models.InPersonVisits;

namespace Visitz.Views.Todo;

#nullable enable

public partial class TodoListViewModel : VisitzViewModel
{
    const bool SortAscending = true;

    [ObservableProperty]
    public partial bool ShowEmptyView { get; set; } = true;

    Realm? DataRealm { get; set; }

    readonly ObservableRealmQueryMap _queryMap = new();

    readonly ObservableCollection<PersonVisit> _allVisits = [];
    readonly ObservableCollection<PersonVisit> _filteredVisits = [];

    public ObservableCollection<ITodoItem> AllTodoItems { get; } = [];

    protected override async Task InitAsync()
    {
        await base.InitAsync();

        _allVisits.CollectionChanged += AllVisits_CollectionChanged;
        _filteredVisits.CollectionChanged += SupportingList_CollectionChanged;
        AllTodoItems.CollectionChanged += TodoItems_CollectionChanged;

        DataRealm = await VisitzRealms.GetIcmDataRealmAsync();

        _queryMap.ItemsChanged += QueryMap_ItemsChanged;
        _queryMap.Subscribe(DataRealm, PersonVisit.GetLatestVisitsPerParentRecord(DataRealm));
    }

    bool disposed;

    protected override void Dispose(bool disposing)
    {
        if (!disposed && disposing)
        {
            _queryMap.Dispose();
            DataRealm = null;

            _queryMap.ItemsChanged -= QueryMap_ItemsChanged;
            _allVisits.CollectionChanged -= AllVisits_CollectionChanged;
            _filteredVisits.CollectionChanged -= SupportingList_CollectionChanged;
            AllTodoItems.CollectionChanged -= TodoItems_CollectionChanged;

            disposed = true;
        }
        base.Dispose(disposing);
    }

    void QueryMap_ItemsChanged(object? sender, (Type Type, IRealmCollection<IRealmObject> Items, ChangeSet? Changes) e)
    {
        if (e.Type == typeof(PersonVisit))
            UpdateSupportingList(e.Items, e.Changes, _allVisits);
    }

    /// <summary>
    /// Processes incoming results from a realm query to insert into a supporting list for the main collections.
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    /// <param name="items"></param>
    /// <param name="changes"></param>
    /// <param name="draftsList"></param>
    /// <param name="mapper"></param>
    void UpdateSupportingList<TItem>(
        IRealmCollection<IRealmObject> items,
        ChangeSet? changes,
        ObservableCollection<TItem> draftsList
    )
        where TItem : IRealmObject
    {
        if (changes == null)
            draftsList.AddAll(items.Cast<TItem>());
        else
        {
            foreach (int deleteIndex in changes.DeletedIndices.Reverse())
                draftsList.RemoveAt(deleteIndex);

            foreach (int insertIndex in changes.InsertedIndices)
                draftsList.Add((TItem)items.ElementAt(insertIndex));

            if (changes.ModifiedIndices.Length > 0)
                AllTodoItems.Sort(ascending: SortAscending);
        }
    }

    /// <summary>
    /// Processes explicit add/remove items into a supporting list for the main ObservableCollection 'TodoItems'.
    /// </summary>
    /// <typeparam name="VM"></typeparam>
    /// <param name="addItems"></param>
    /// <param name="removeItems"></param>
    /// <param name="draftsList"></param>
    /// <param name="mapper"></param>
    static void UpdateSupportingList<VM>(
        IEnumerable<IRealmObject> addItems,
        IEnumerable<VM> removeItems,
        ObservableCollection<VM> draftsList,
        Func<IRealmObject, VM> mapper
    )
    {
        foreach (var remove in removeItems)
            draftsList.Remove(remove);

        foreach (var add in addItems)
            draftsList.Add(mapper(add));
    }

    void TodoItems_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        ShowEmptyView = sender is ObservableCollection<ITodoItem> { Count: <= 0 };
    }

    void AllVisits_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems != null)
        {
            foreach (var visit in e.NewItems.Cast<PersonVisit>())
                if (PersonVisit.IsUpcomingVisit(visit))
                    _filteredVisits.Add(visit);
        }
        else if (e.Action == NotifyCollectionChangedAction.Remove && e.OldItems != null)
        {
            foreach (var visit in e.OldItems.Cast<PersonVisit>())
                _filteredVisits.Remove(visit);
        }
    }

    void SupportingList_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems != null)
        {
            foreach (var item in e.NewItems)
                AllTodoItems.InsertSorted(MakeTodoItem(item), ascending: SortAscending);
        }
        else if (e.Action == NotifyCollectionChangedAction.Remove && e.OldItems != null)
        {
            foreach (var item in e.OldItems)
            {
                if (AllTodoItems.FirstOrDefault(todoItem => todoItem.Item == item) is ITodoItem todoItem)
                {
                    AllTodoItems.Remove(todoItem);

                    if (todoItem is IDisposable disposable)
                        disposable.Dispose();
                }
            }
        }
    }

    // TODO: return type as ITodoItem once we support multiple types in this list
    static TodoVisitItemViewModel MakeTodoItem(object item)
    {
        if (item is PersonVisit visit)
            return new TodoVisitItemViewModel(visit);
        else
            throw new InvalidOperationException($"Type '{item.GetType()}' not supported");
    }
}
