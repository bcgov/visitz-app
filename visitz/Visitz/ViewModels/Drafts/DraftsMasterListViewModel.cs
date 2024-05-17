using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Realms;
using System.Collections.ObjectModel;
using Visitz.Resources.Localization;
using Visitz.Storage;
using VisitzModel.Messaging;
using VisitzModel.Models;
using VisitzModel.Models.Drafts;
using VisitzModel.Models.SafetyAssess;

namespace Visitz.ViewModels.Drafts;

internal partial class DraftsMasterListViewModel : VisitzViewModel
{
	[ObservableProperty]
	public ObservableCollection<object> masterDraftItems = [];

	[ObservableProperty]
	public MasterDraftItem selectedItem;

	[ObservableProperty]
	public bool showEmptyView;

	readonly ObservableRealmCount realmCount = new();

	MasterDraftItem noteDraftItem;

	MasterDraftItem assessmentDraftItem;

	public override async void Create()
	{
		base.Create();

		realmCount.CountChanged += RealmCount_CountChanged;

		realmCount.Subscribe<NoteDraft>(await VisitzRealms.GetNoteDraftsRealmAsync());
		realmCount.Subscribe<SafetyAssessment>(await VisitzRealms.GetSafetyAssessmentDraftRealmAsync());
	}

	public override void Destroy()
	{
		base.Destroy();

		realmCount.CountChanged -= RealmCount_CountChanged;
		realmCount.Dispose();
	}

	private void RealmCount_CountChanged(object sender, (Type Kind, int Count) e)
	{
		ShowEmptyView = (sender as ObservableRealmCount).Total <= 0;

		if (e.Kind == typeof(NoteDraft))
			UpdateItem(LocalizedStrings.Notes, e.Count, ref noteDraftItem, typeof(NoteDraft));
		else if (e.Kind == typeof(SafetyAssessment))
			UpdateItem(LocalizedStrings.SafetyAssessment, e.Count, ref assessmentDraftItem, typeof(SafetyAssessment));
	}

	void UpdateItem(string name, int count, ref MasterDraftItem item, Type itemType)
	{
		if (item != null)
			MasterDraftItems.Remove(item);

		if (count > 0)
		{
			item = new MasterDraftItem()
			{
				Name = name,
				Count = count,
				ItemType = itemType,
			};

			InsertSortedAsc(MasterDraftItems, item);
		}
	}

	[RelayCommand]
	public void MasterDraftItemSelected()
	{
		var kind = SelectedItem.ItemType;
		var msg = new DraftMasterSelectedMessage(kind, realmCount[kind].Realm);
		StrongReferenceMessenger.Default.Send(msg);
	}

	// TODO: Use the IList<T>.InsertSortedAsc once MAUI fixes ObservableCollection<object> issues.
	// https://github.com/dotnet/maui/issues/8435#issuecomment-1365586648
	static void InsertSortedAsc(ObservableCollection<object> collection, MasterDraftItem newDraft)
	{
		if (collection.Count == 0)
			collection.Add(newDraft);
		else
		{
			var find = collection.FirstOrDefault(obj => (obj as MasterDraftItem).CompareTo(newDraft) >= 0);
			if (find != null)
				collection.Insert(collection.IndexOf(find), newDraft);
			else
				collection.Add(newDraft);
		}
	}
}
