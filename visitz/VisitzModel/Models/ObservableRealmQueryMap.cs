using Realms;

namespace VisitzModel.Models;

public class ObservableRealmQueryMap<T> : IDisposable where T : IRealmObject
{
	private bool disposedValue;

	Dictionary<Type, (Realm, IQueryable, IDisposable QueryToken)> Queries { get; } = [];

	public event EventHandler<(Type, IRealmCollection<T> Items, ChangeSet Changes)> ItemsChanged;

	public void Subscribe(Realm realm, IQueryable<T> query)
	{
		var queryToken = query.SubscribeForNotifications(Query_ItemsChanged);
		Queries[typeof(T)] = (realm, query, queryToken);
	}

	public void Unsubscribe<Q>() where Q : T
	{
		if (Queries.ContainsKey(typeof(Q)))
		{
			Queries[typeof(Q)].QueryToken.Dispose();
			Queries.Remove(typeof(Q));
		}
	}

	public void UnsubscribeAll()
	{
		foreach (var (_, _, token) in Queries.Values)
			token.Dispose();

		Queries.Clear();
	}

	void Query_ItemsChanged(IRealmCollection<T> sender, ChangeSet changes)
	{
		ItemsChanged?.Invoke(this, (typeof(T), sender, changes));
	}

	protected virtual void Dispose(bool disposing)
	{
		if (!disposedValue)
		{
			if (disposing)
				UnsubscribeAll();

			disposedValue = true;
		}
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}
}
