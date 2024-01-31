using Realms;

namespace Visitz.Extensions;

internal static class RealmExtensions
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
}
