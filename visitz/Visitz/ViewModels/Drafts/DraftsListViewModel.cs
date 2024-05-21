using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Realms;
using System.Collections.ObjectModel;
using VisitzModel.Messaging;
using VisitzModel.Models;

namespace Visitz.ViewModels.Drafts;

internal partial class DraftsListViewModel : VisitzViewModel
{
	[ObservableProperty]
	public ObservableCollection<object> draftItems = [];

	readonly ObservableRealmQueryMap queryMap = new();

	public override void Create()
	{
		base.Create();

		queryMap.ItemsChanged += QueryMap_ItemsChanged;

		StrongReferenceMessenger.Default.Register<DraftMasterSelectedMessage>(this, DraftMasterSelected);
	}

	public override void Destroy()
	{
		base.Destroy();

		StrongReferenceMessenger.Default.UnregisterAll(this);

		queryMap.ItemsChanged -= QueryMap_ItemsChanged;
	}

	private void DraftMasterSelected(object _, DraftMasterSelectedMessage message)
	{
		queryMap.UnsubscribeAll();

		var (type, realm) = message.Value;

		if (type == typeof(NoteDraft))
			queryMap.Subscribe(realm, realm.All<NoteDraft>());
	}

	private void QueryMap_ItemsChanged(object _, (Type, IRealmCollection<IRealmObject> Items, ChangeSet Changes) e)
	{
		if (e.Changes == null)
		{
			DraftItems.Clear();

			foreach (var item in e.Items)
				DraftItems.Add(item);
		}
	}
}
