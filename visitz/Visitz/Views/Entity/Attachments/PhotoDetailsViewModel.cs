using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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
using VisitzModel.Storage.Filesystem;

namespace Visitz.Views.Entity.Attachments;

internal partial class PhotoDetailsViewModel : VisitzViewModel, ICaseloadItemHolder
{
    [ObservableProperty]
    public Attachment attachment;

    [ObservableProperty]
    public ImageSource detailImage;

    [ObservableProperty]
    public CaseloadItem caseloadItem;

    AttachmentFiler attachmentFiler;

    protected override async Task InitAsync()
    {
        await base.InitAsync();

        attachmentFiler = await VisitzFiles.GetAsync(
            CaseloadItem.EntityType.ParseEntityType(),
            CaseloadItem.CaseIncidentNumber,
            CaseloadItem.KeyPlayer.FirstName,
            CaseloadItem.KeyPlayer.LastName);

        DetailImage = ImageSource.FromStream(GetPhoto);
    }

    async Task<Stream> GetPhoto(CancellationToken token)
    {
        return await attachmentFiler.GetAppDataFileAsync(Attachment.RelativePath, token);
    }

    [RelayCommand]
    public static void DeleteAttachmentDraft(Attachment attachment)
    {
        if (attachment.HasDraft)
            _ = PromptDiscardAttachmentAsync(attachment);
    }

    static async Task PromptDiscardAttachmentAsync(Attachment attachment)
    {
        bool shouldDiscard = await Navigator.CurrentOpenPage.DisplayAlert(
            LocalizedStrings.DiscardDraft,
            LocalizedStrings.DiscardAttachmentDraftDescription,
            LocalizedStrings.Discard,
            LocalizedStrings.Cancel);

        if (shouldDiscard)
        {
            string filename = attachment.Filename;

            await attachment.DeleteAsync();
            await Navigator.Navigation.PopAsync();

            SnackbarHandler.ShowText(LocalizedStrings.FileDiscarded.Format(filename));
        }
    }

    [RelayCommand]
    public void PublishAttachmentDraft(Attachment attachment)
    {
        if (attachment.HasDraft)
            _ = DoPublishAttachmentDraft(attachment.Draft);
    }

    async Task DoPublishAttachmentDraft(AttachmentDraft draft)
    {
        var attachmentPublishVm = ServiceProvider.Current.GetService<AttachmentDraftPublishViewModel>();
        attachmentPublishVm.AttachmentDraft = draft;
        attachmentPublishVm.AttachmentFiler = attachmentFiler;

        await Navigator.Navigation.PushModalAsync(new PublishPage(attachmentPublishVm));
    }
}
