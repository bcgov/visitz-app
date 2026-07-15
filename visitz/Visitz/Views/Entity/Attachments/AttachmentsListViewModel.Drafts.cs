using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Visitz.Resources.Localization;
using Visitz.Views.BaseClasses.Publishing;
using Visitz.Views.Snackbar;
using VisitzModel.Models.Attachments;

namespace Visitz.Views.Entity.Attachments;

public partial class AttachmentsListViewModel
{
    private const string _noAttachmentError = "Selected draft does not have an attachment";

    [RelayCommand]
    public static void DeleteAttachmentDraft(AttachmentDraft draft)
    {
        _ = PromptDiscardAttachmentDraftAsync(draft);
    }

    static async Task PromptDiscardAttachmentDraftAsync(AttachmentDraft draft)
    {
        if (draft.Attachment == null)
            throw new InvalidOperationException(_noAttachmentError);

        bool shouldDiscard = await Navigator.CurrentOpenPage.DisplayAlertAsync(
            LocalizedStrings.DiscardDraft,
            LocalizedStrings.DiscardAttachmentDraftDescription,
            LocalizedStrings.Discard,
            LocalizedStrings.Cancel
        );

        if (shouldDiscard)
        {
            string filename = draft.Attachment.Filename;
            await draft.Attachment.DeleteAsync();
            SnackbarHandler.ShowText(string.Format(LocalizedStrings.FileDiscarded, filename));
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
        var logger = ServiceProvider.GetService<ILogger<PublishPage>>();
        if (attachmentPublishVm == null)
            return;

        await attachmentPublishVm.SetPayload(BusinessObject, draft);
        await Navigator.Navigation.PushModalAsync(new PublishPage(attachmentPublishVm, logger));
    }

    [RelayCommand]
    public static void RenameAttachment(Attachment attachment)
    {
        _ = DoRenameAttachment(attachment);
    }

    static async Task DoRenameAttachment(Attachment attachment)
    {
        string previous = attachment.FilenameBinding;

        string newName = await Navigator.CurrentOpenPage.DisplayPromptAsync(
            LocalizedStrings.Rename,
            null,
            placeholder: previous,
            initialValue: previous
        );

        if (newName != previous && newName?.Trim()?.Length > 0)
            attachment.FilenameBinding = newName;
    }
}
