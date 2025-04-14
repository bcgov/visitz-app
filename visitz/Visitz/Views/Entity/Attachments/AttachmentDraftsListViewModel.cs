using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Realms;
using System.Collections.ObjectModel;
using Visitz.Extensions;
using Visitz.Resources.Localization;
using Visitz.Storage;
using Visitz.Views.BaseClasses;
using Visitz.Views.BaseClasses.Publishing;
using Visitz.Views.Snackbar;
using VisitzModel.Extensions;
using VisitzModel.Extensions.EntityTypes;
using VisitzModel.Interfaces;
using VisitzModel.Models;
using VisitzModel.Models.Attachments;

namespace Visitz.Views.Entity.Attachments;

internal partial class AttachmentDraftsListViewModel : VisitzViewModel, ICaseloadItemHolder
{
	[ObservableProperty]
	public CaseloadItem caseloadItem;

	Realm attachmentsRealm;

	readonly ObservableRealmQueryMap realmQuery = new();

	[ObservableProperty]
	ObservableCollection<AttachmentDraft> attachmentDrafts = [];

	[ObservableProperty]
	public bool isLoading = true;

	[ObservableProperty]
	public bool isEmpty;

	public readonly TaskCompletionSource attachmentsLoadedTcs = new();

    protected override async Task InitAsync()
    {
        await base.InitAsync();

		attachmentsRealm = await VisitzRealms.GetAttachmentDraftsRealmAsync();

		realmQuery.Subscribe(attachmentsRealm, attachmentsRealm.All<AttachmentDraft>()
				.Where(draft => draft.RelatedEntityId == CaseloadItem.CaseIncidentNumber));

		realmQuery.ItemsChanged += RealmQuery_ItemsChanged;
    }

    bool disposed;
    protected override void Dispose(bool disposing)
    {
        if (!disposed && disposing)
        {
		    realmQuery.ItemsChanged -= RealmQuery_ItemsChanged;
		    realmQuery.Dispose();
		    attachmentsRealm?.Dispose();

            disposed = true;
        }
        base.Dispose(disposing);
    }

    private void RealmQuery_ItemsChanged(object sender, (Type Type, IRealmCollection<IRealmObject> Items, ChangeSet Changes) e)
	{
		IsLoading = false;
		IsEmpty = !realmQuery[typeof(AttachmentDraft)].Query.Any();

		if (e.Changes == null)
		{
			foreach (var item in e.Items)
				AttachmentDrafts.Add(item as AttachmentDraft);

			attachmentsLoadedTcs.TrySetResult();
		}
		else
		{
			foreach (int deleted in e.Changes.DeletedIndices.Reverse())
				AttachmentDrafts.RemoveAt(deleted);

			foreach (int modified in e.Changes.ModifiedIndices)
				AttachmentDrafts[modified] = e.Items[modified] as AttachmentDraft;

			foreach (int inserted in e.Changes.InsertedIndices)
				AttachmentDrafts.Insert(inserted, e.Items[inserted] as AttachmentDraft);
		}
	}

	[RelayCommand]
	public static void DeleteAttachmentDraft(AttachmentDraft draft)
	{
		_ = PromptDiscardAttachmentDraftAsync(draft);
	}

	static async Task PromptDiscardAttachmentDraftAsync(AttachmentDraft draft)
	{
		bool shouldDiscard = await Navigator.CurrentOpenPage.DisplayAlert(
			LocalizedStrings.DiscardDraft,
			LocalizedStrings.DiscardAttachmentDraftDescription,
			LocalizedStrings.Discard,
			LocalizedStrings.Cancel);

		if (shouldDiscard)
		{
			string filename = draft.Attachment.Filename;
			await draft.Attachment.DeleteAsync();
			SnackbarHandler.ShowText(LocalizedStrings.FileDiscarded.Format(filename));
		}
	}

	[RelayCommand]
	public void PublishAttachmentDraft(AttachmentDraft draft)
	{
		_ = DoPublishAttachmentDraft(draft);
	}

	async Task DoPublishAttachmentDraft(AttachmentDraft draft)
	{
		var attachmentPublishVm = ServiceProvider.Current.GetService<AttachmentDraftPublishViewModel>();
		attachmentPublishVm.AttachmentDraft = draft;
		attachmentPublishVm.AttachmentFiler = await VisitzFiles.GetAsync(
			CaseloadItem.EntityType.ParseEntityType(),
			CaseloadItem.CaseIncidentNumber,
			CaseloadItem.KeyPlayer.FirstName,
			CaseloadItem.KeyPlayer.LastName);

		await Navigator.Navigation.PushModalAsync(new PublishPage(attachmentPublishVm));
	}

	[RelayCommand]
	public void OpenAttachment(AttachmentDraft draft)
	{
		_ = DoOpenAttachment(draft);
	}

	async Task DoOpenAttachment(AttachmentDraft draft)
	{
		var view = new PhotoDetailsView()
		{
			Attachment = draft.Attachment,
			CaseloadItem = CaseloadItem,
		}.WrapPageForModal(ViewModalSize.Fullscreen);

		await Navigator.Navigation.PushAsync(view);
	}
}
