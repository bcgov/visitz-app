using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Realms;
using System.Collections.ObjectModel;
using Visitz.Storage;
using Visitz.Views.Caseload;
using VisitzModel.Messaging;
using VisitzModel.Models;
using VisitzModel.Models.Drafts;
using VisitzModel.Models.Navigation;
using VisitzModel.Models.SafetyAssess;

namespace Visitz.ViewModels.Drafts;

internal partial class DraftsListViewModel : VisitzViewModel
{
	[ObservableProperty]
	public ObservableCollection<object> draftItems = [];

	readonly ObservableRealmQueryMap queryMap = new();

	Realm DataRealm { get; set; }

	EntitySection SectionToOpen { get; set; }

	public override async void Create()
	{
		base.Create();

		DataRealm = await VisitzRealms.GetIcmDataRealmAsync();

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
		{
			SortAndSubscribe(realm, realm.All<NoteDraft>());
			SectionToOpen = EntitySection.NoteEntry;
		}
		else if (type == typeof(AssessmentDraft))
		{
			SortAndSubscribe(realm, realm.All<AssessmentDraft>());
			SectionToOpen = EntitySection.SafetyAssessment;
		}
		else
			throw new InvalidOperationException($"Type {type} not supported in Drafts view.");
	}

	private void SortAndSubscribe<T>(Realm realm, IQueryable<T> query) where T : IRealmObject
	{
		var sortedQuery = query.Filter($"TRUEPREDICATE SORT({nameof(IDraftItem.LastUpdated)} DESC)");
		queryMap.Subscribe(realm, sortedQuery);
	}

	private void QueryMap_ItemsChanged(object _, (Type, IRealmCollection<IRealmObject> Items, ChangeSet Changes) e)
	{
		DraftItems.Clear();

		foreach (var item in e.Items)
			DraftItems.Add(item);
	}

	[RelayCommand]
	private void DraftItemSelected(IDraftItem draftItem)
	{
		var caseloadItem = DataRealm
			.All<CaseloadItem>()
			.Where(item => item.CaseIncidentNumber == draftItem.RelatedEntityId)
			.FirstOrDefault();

		NavigateTo(caseloadItem, SectionToOpen);
	}

	static void NavigateTo(CaseloadItem caseloadItem, EntitySection section)
	{
		var caseloadNav = new CaseloadItemSelectedMessage(caseloadItem, section);
		StrongReferenceMessenger.Default.Send(caseloadNav);

		var appNav = new AppNavMessage(new() { ContentViewType = typeof(CaseloadContainerView) });
		StrongReferenceMessenger.Default.Send(appNav);
	}
}
