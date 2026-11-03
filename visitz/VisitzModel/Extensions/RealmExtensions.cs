using Realms;

namespace VisitzModel.Extensions;

public static class RealmExtensions
{
    public static void Commit(this IRealmObject realmObject, Action action)
    {
        if (!realmObject.IsManaged || realmObject.Realm.IsInTransaction)
            action.Invoke();
        else
            realmObject.Realm.Write(action);
    }

    public static async Task CommitAsync(this IRealmObject realmObject, Action action)
    {
        if (!realmObject.IsManaged || realmObject.Realm.IsInTransaction)
            action.Invoke();
        else
            await realmObject.Realm.WriteAsync(action);
    }

    public static async Task CommitAsync(this Realm realm, Action action)
    {
        if (realm.IsInTransaction)
            action.Invoke();
        else
            await realm.WriteAsync(action);
    }

    public static void Upsert<T>(this Realm realm, T item)
        where T : IRealmObject
    {
        realm.Add(item, update: true);
    }

    public static void Upsert<T>(this Realm realm, IEnumerable<T> enumerable)
        where T : IRealmObject
    {
        foreach (var item in enumerable)
            realm.Add(item, update: true);
    }

    public static void DeleteByIds<T>(this Realm realm, IEnumerable<string> ids)
        where T : IRealmObject
    {
        foreach (var id in ids)
            if (realm.Find<T>(id) is T found)
                realm.Remove(found);
    }

    public delegate void UpdateMapper<TSource>(TSource existing, TSource incoming);

    /// <summary>
    /// <para>
    /// Synchronizes a new (incoming) collection of items with a scoped query of existing items from a local Realm
    /// database.
    /// </para>
    ///
    /// <para>
    /// "Synchronizing" in this context means deleting existing items not present in the
    /// incoming data, updating common items with new info, and adding new items that don't currently exist in
    /// the query.
    /// </para>
    ///
    /// <para>
    /// "Scoped" specifically means <b>existingQuery</b> should be intentionally limited to a specific range of items
    /// that are relevant to <b>incomingItems</b>. Scope is typically all items for a single parent record, like all
    /// Notes for a given Case, but is not required to be so.
    /// </para>
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <param name="realm">The database to synchronize.</param>
    /// <param name="incomingItems">New TSource items to be synchronized into the database in relation to
    /// <b>existingQuery</b>.</param>
    /// <param name="existingQuery">A scoped range of already-existing TSource objects in the local database that
    /// <b>incomingItems</b> uses for set operations.</param>
    /// <param name="equalityComparer">Used to specify equality for set operations between <b>incomingItems</b> and
    /// <b>existingQuery</b>. Remember to provide an implementation that also implements GetHashCode. Uses default if
    /// null.</param>
    /// <param name="beforeDelete">Used for running operations on a TSource object before it's
    /// removed from the provided realm.</param>
    /// <param name="updateMapper">Used to explicitly map TSource properties from incoming ➡️ existing.
    /// If null, TSource is upserted instead.</param>
    /// <param name="updateComparer">Used for sorting and binary searching <b>existingQuery</b>'s results to run
    /// <b>updateMapper</b> against. TSource's default comparer is used if null. Only used if <b>updateMapper</b> is
    /// not null.</param>
    /// <returns></returns>
    public static async Task SynchronizeByQueryAsync<TSource>(
        this Realm realm,
        IEnumerable<TSource> incomingItems,
        IEnumerable<TSource> existingQuery,
        IEqualityComparer<TSource>? equalityComparer = null,
        Action<TSource>? beforeDelete = null,
        UpdateMapper<TSource>? updateMapper = null,
        IComparer<TSource>? updateComparer = null
    )
        where TSource : IRealmObject
    {
        // Issues with Realm object lifetime and deferred execution, so materialize everything to
        // lists instead
        List<TSource> currentItems = existingQuery.ToList();

        if (currentItems.Count == 0 && !incomingItems.Any())
            return;

        if (updateMapper != null || updateComparer != null)
            currentItems.Sort(updateComparer);

        List<TSource> deleteItems = currentItems.Except(incomingItems, equalityComparer).ToList();
        List<TSource> insertItems = incomingItems.Except(currentItems, equalityComparer).ToList();
        List<TSource> updateItems = incomingItems.Intersect(currentItems, equalityComparer).ToList();

        await realm.CommitAsync(() =>
        {
            foreach (TSource deleteItem in deleteItems)
            {
                beforeDelete?.Invoke(deleteItem);
                realm.Remove(deleteItem);
            }

            foreach (TSource insertItem in insertItems)
                realm.Add(insertItem);

            foreach (TSource updateItem in updateItems)
            {
                if (updateMapper != null)
                {
                    int index = currentItems.BinarySearch(updateItem, updateComparer);
                    if (index < 0)
                        // If not found, it means the item is either deleted or new.
                        // Skip update mapping.
                        continue;

                    TSource currentItem = currentItems[index];

                    // No explicit realm call, since we're updating the currentItem object itself
                    // with the updateMapper. This avoids Realm sending an "Object added"
                    // notification, since we're only updating its properties.
                    updateMapper.Invoke(currentItem, updateItem);
                }
                else
                    // No updateMapper here so just upsert the object. We could add external
                    // dependencies for auto-mapping but I don't think it's worth it in this case.
                    // Unfortunately, this means realm will send "Object added" notifications for
                    // these, even though they're only updated.
                    realm.Add(updateItem, update: true);
            }
        });
    }
}
