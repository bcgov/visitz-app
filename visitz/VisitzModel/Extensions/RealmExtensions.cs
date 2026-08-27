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
}
