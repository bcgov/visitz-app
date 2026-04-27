using Realms;

namespace VisitzModel.Models;

public class ObservableRealmCount : IDisposable
{
    private bool disposedValue;

    public Dictionary<Type, (Realm Realm, IQueryable, IDisposable Token, int Count)> CountSubscriptions { get; } = [];

    public event EventHandler<(Type Kind, int Count)>? CountChanged;

    public (Realm Realm, IQueryable Query, IDisposable Token, int Count) this[Type key] => CountSubscriptions[key];

    public int Total
    {
        get
        {
            int total = 0;

            foreach (var (_, _, _, count) in CountSubscriptions.Values)
                total += count;

            return total;
        }
    }

    public void Subscribe<T>(Realm realm)
        where T : IRealmObject
    {
        var query = realm.All<T>();
        var queryToken = query.SubscribeForNotifications(CountRecords);
        CountSubscriptions[typeof(T)] = (realm, query, queryToken, 0);
    }

    void CountRecords<T>(IRealmCollection<T> sender, ChangeSet? _)
    {
        var (realm, query, token, _) = CountSubscriptions[typeof(T)];
        CountSubscriptions[typeof(T)] = (realm, query, token, sender.Count);

        CountChanged?.Invoke(this, (typeof(T), sender.Count));
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                foreach (var (_, _, token, _) in CountSubscriptions.Values)
                    token.Dispose();

                CountSubscriptions.Clear();
            }

            disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
