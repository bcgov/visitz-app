using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Realms;
using System.Collections.ObjectModel;
using Visitz.Resources.Localization;
using Visitz.Storage;
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

	MasterDraftItem noteDraftItem;
	IQueryable<NoteDraft> noteDraftQuery;
	IDisposable noteQueryToken;

	MasterDraftItem assessmentDraftItem;
	IQueryable<SafetyAssessment> safetyAssessmentDraftQuery;
	IDisposable assessmentQueryToken;

	public override async void Create()
	{
		base.Create();

		var noteDraftsRealm = await VisitzRealms.GetNoteDraftsRealmAsync();
		noteDraftQuery = noteDraftsRealm.All<NoteDraft>();
		noteQueryToken = noteDraftQuery.SubscribeForNotifications(NoteDraftsCount);

		var assessmentDraftsRealm = await VisitzRealms.GetSafetyAssessmentDraftRealmAsync();
		safetyAssessmentDraftQuery = assessmentDraftsRealm.All<SafetyAssessment>();
		assessmentQueryToken = safetyAssessmentDraftQuery.SubscribeForNotifications(SafetyAssessmentDraftsCount);
	}

	public override void Destroy()
	{
		base.Destroy();

		noteQueryToken.Dispose();
		noteDraftQuery = null;

		assessmentQueryToken.Dispose();
		safetyAssessmentDraftQuery = null;
	}

	void NoteDraftsCount(IRealmCollection<NoteDraft> sender, ChangeSet _)
	{
		UpdateItem(LocalizedStrings.Notes, sender.Count, ref noteDraftItem);
	}

	void SafetyAssessmentDraftsCount(IRealmCollection<SafetyAssessment> sender, ChangeSet _)
	{
		UpdateItem(LocalizedStrings.SafetyAssessment, sender.Count, ref assessmentDraftItem);
	}

	void UpdateItem(string name, int count, ref MasterDraftItem item)
	{
		if (item != null)
			MasterDraftItems.Remove(item);

		if (count > 0)
		{
			item = new MasterDraftItem()
			{
				Name = name,
				Count = count,
			};

			InsertSortedAsc(MasterDraftItems, item);
		}
	}

	[RelayCommand]
	public void MasterDraftItemSelected()
	{

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
