using System.Collections.ObjectModel;
using System.Collections.Specialized;
using CommunityToolkit.Mvvm.ComponentModel;
using Realms;
using VisitzModel.Extensions;

namespace VisitzModel.Models;

/// <summary>
/// <para>Contains the lifetime and results of a Realm query. Allows callers to determine how
/// <see cref="TQueryObject"/> Realm objects are mapped into <see cref="TListItem"/> objects
/// for display in UI.</para>
/// <para>Also see <see cref="ObservableQuery{TQueryObject}"/> for a shorthand implementation, if
/// applicable.</para>
/// </summary>
/// <typeparam name="TQueryObject">The Realm object queried from the database.</typeparam>
/// <typeparam name="TListItem">The type intended to be used in the UI. If it has the same type as
/// <see cref="TQueryObject"/>, use <see cref="ObservableQuery{TQueryObject}"/> instead.</typeparam>
public partial class ObservableQuery<TQueryObject, TListItem> : ObservableObject, IDisposable
    where TQueryObject : IRealmObject
{
    bool _disposedValue;

    public Realm Realm { get; }

    public IQueryable<TQueryObject> RealmQuery { get; private set; }

    public IDisposable QueryToken { get; }

    ObservableCollection<TQueryObject> QueryItems { get; } = [];

    [ObservableProperty]
    public partial ObservableCollection<TListItem> Items { get; private set; } = [];

    public delegate void ItemMapperDelegate(TQueryObject itemToMap, ObservableCollection<TListItem> targetList);

    public ItemMapperDelegate AddItemMapper { get; }

    public ItemMapperDelegate RemoveItemMapper { get; }

    [ObservableProperty]
    public partial bool HasAnyItems { get; private set; }

    public ObservableQuery(
        Realm realm,
        IQueryable<TQueryObject> query,
        ItemMapperDelegate addItem,
        ItemMapperDelegate removeItem
    )
    {
        Realm = realm;
        RealmQuery = query;
        AddItemMapper = addItem;
        RemoveItemMapper = removeItem;

        QueryItems.CollectionChanged += QueryItems_CollectionChanged;
        QueryToken = query.SubscribeForNotifications(Query_ItemsChanged);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                QueryToken.Dispose();
                RealmQuery = Enumerable.Empty<TQueryObject>().AsQueryable();

                QueryItems.CollectionChanged -= QueryItems_CollectionChanged;
                QueryItems.Clear();
                Items.Clear();

                Realm.Dispose();
            }

            _disposedValue = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    void Query_ItemsChanged(IRealmCollection<TQueryObject> queriedItems, ChangeSet? changes)
    {
        if (changes == null)
            QueryItems.AddAll(queriedItems);
        else
        {
            foreach (int deletedIndex in changes.DeletedIndices.Reverse())
            {
                if (QueryItems[deletedIndex] is IDisposable disposable)
                    disposable.Dispose();

                QueryItems.RemoveAt(deletedIndex);
            }

            foreach (int insertedIndex in changes.InsertedIndices)
                QueryItems.Add(queriedItems[insertedIndex]);
        }
    }

    void QueryItems_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems != null)
        {
            foreach (TQueryObject newItem in e.NewItems.Cast<TQueryObject>())
                AddItemMapper(newItem, Items);
        }
        else if (e.Action == NotifyCollectionChangedAction.Remove && e.OldItems != null)
        {
            foreach (TQueryObject newItem in e.OldItems.Cast<TQueryObject>())
                RemoveItemMapper(newItem, Items);
        }
        else if (e.Action == NotifyCollectionChangedAction.Reset)
        {
            Items.Clear();
        }

        HasAnyItems = Items.Any();
    }
}

/// <summary>
/// "Shorthand" <see cref="ObservableQuery{TQueryObject, TListItem}"/> if both generic arguments are the same type.
/// </summary>
/// <typeparam name="TQueryObject">The type to be used both from the query and display in the UI.</typeparam>
public partial class ObservableQuery<TQueryObject> : ObservableQuery<TQueryObject, TQueryObject>
    where TQueryObject : IRealmObject
{
    public ObservableQuery(Realm realm, IQueryable<TQueryObject> query)
        : base(
            realm,
            query,
            (addItem, targetList) => targetList.Add(addItem),
            (removeItem, targetList) => targetList.Remove(removeItem)
        ) { }

    public ObservableQuery(
        Realm realm,
        IQueryable<TQueryObject> query,
        ItemMapperDelegate addItem,
        ItemMapperDelegate removeItem
    )
        : base(realm, query, addItem, removeItem) { }
}
