using CommunityToolkit.Mvvm.Messaging;
using Visitz.Documents;
using Visitz.Resources.Localization;
using Visitz.Services;
using Visitz.Services.Attachments;
using Visitz.Services.Base;
using Visitz.Views.BaseClasses.Publishing;
using VisitzApi.Models.Attachments;
using VisitzModel.Extensions;
using VisitzModel.Interfaces;
using VisitzModel.Models;
using VisitzModel.Models.Attachments;
using VisitzModel.Storage.Filesystem;

namespace Visitz.Views.Entity.Attachments;

internal class AttachmentDraftPublishViewModel : PublishViewModel, IRecipient<ServiceStateMessage>, ICaseloadItemHolder
{
    AttachmentDraft attachmentDraft;

    public AttachmentDraft AttachmentDraft
    {
        set
        {
            attachmentDraft = value;

            Title = AttachmentName;
        }
    }

    SubmitAttachmentEntity submitEntity;

    string getAttachmentsServiceId;
    string submitAttachmentsServiceId;

    public CaseloadItem CaseloadItem { get; set; }

    string AttachmentName => attachmentDraft.Attachment.Filename;

    public AttachmentFiler AttachmentFiler { get; set; }

    public override async void Create()
    {
        base.Create();

        var converter = TryMakeImageToPdfConverter(attachmentDraft);
        submitEntity = await attachmentDraft.ToSubmitAttachmentEntity(AttachmentFiler, converter);

        getAttachmentsServiceId = GetAttachmentsService.MakeId(
            attachmentDraft.RelatedEntityType,
            CaseloadItem.RowId);
        submitAttachmentsServiceId = SubmitAttachmentService.MakeId(submitEntity);

        WeakReferenceMessenger.Default.Register(this, submitAttachmentsServiceId);
        WeakReferenceMessenger.Default.Register(this, getAttachmentsServiceId);

        Wait(LocalizedStrings.LoginToSubmitAttachment);

        Publish();
    }

    public override void Destroy()
    {
        base.Destroy();

        WeakReferenceMessenger.Default.UnregisterAll(this);
    }

    public override void Publish()
    {
        var startMessage = SubmitAttachmentService.MakeStartMessage(submitEntity);
        WeakReferenceMessenger.Default.Send(startMessage);
    }

    private void CallGetService()
    {
        var recordServiceInfo = new RecordServiceInfo(
                attachmentDraft.RelatedEntityType,
                CaseloadItem.RowId,
                attachmentDraft.Attachment.FileNumber,
                CaseloadItem.KeyPlayer.FirstName,
                CaseloadItem.KeyPlayer.LastName);
        var startMessage = GetAttachmentsService.MakeStartMessage(recordServiceInfo);
        WeakReferenceMessenger.Default.Send(startMessage);
    }

    public async void Receive(ServiceStateMessage message)
    {
        if (message.ServiceId == submitAttachmentsServiceId)
        {
            if (message.Status == VisitzService.State.Running)
                Publishing(LocalizedStrings.PublishingAttachmentToIcm.Format(AttachmentName));
            else if (message.FinishedSuccess)
            {
                Published(LocalizedStrings.AttachmentPublishSuccess.Format(AttachmentName));
                CallGetService();
                await DiscardAttachmentDraft();
            }
            else if (message.FinishedCancelled)
                Cancel(LocalizedStrings.LoginToSubmitAttachment);
            else if (message.FinishedError)
                PublishError(LocalizedStrings.FailedToPublishToIcm, message.Message);
        }
        else if (message.ServiceId == getAttachmentsServiceId)
        {
            if (message.Status == VisitzService.State.Running)
                Refreshing(LocalizedStrings.RefreshingAttachments);
            else if (message.FinishedSuccess)
            {
                Refreshed(LocalizedStrings.RefreshedAttachmentsOnDevice);
                Complete();
            }
            else if (message.FinishedError)
                RefreshError(LocalizedStrings.FailedToRefreshAttachments, message.Message);
        }
    }

    async Task DiscardAttachmentDraft()
    {
        await attachmentDraft.Attachment.DeleteAsync();
    }

    static ImagePdfStreamConverter TryMakeImageToPdfConverter(AttachmentDraft attachmentDraft)
    {
        return Attachment.AllowedImageTypes.Contains(attachmentDraft.Attachment.Extension)
            ? new ImagePdfStreamConverter(attachmentDraft.Attachment.Filename, DisplayOrientation.Unknown)
            : null;
    }
}
