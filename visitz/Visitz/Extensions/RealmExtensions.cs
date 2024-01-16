using Realms;

namespace Visitz.Extensions;

internal static class RealmExtensions
{
    public static void Commit(this IRealmObject realmObject, Action action)
    {
        if (realmObject.IsManaged)
            realmObject.Realm.Write(action);
        else
            action.Invoke();
    }

    public static async Task CommitAsync(this IRealmObject realmObject, Action action)
    {
        if (realmObject.IsManaged)
            await realmObject.Realm.WriteAsync(action);
        else
            action.Invoke();
    }
}
