using CommunityToolkit.Mvvm.Messaging;
using Realms;
using Visitz.Resources.Localization;
using Visitz.Services;
using Visitz.Services.Attachments;
using Visitz.Services.Base;
using Visitz.Storage;
using Visitz.Views.BaseClasses.Publishing;
using VisitzApi.Models.Attachments;
using VisitzModel.Extensions;
using VisitzModel.Extensions.EntityTypes;
using VisitzModel.Interfaces;
using VisitzModel.Models;
using VisitzModel.Models.Attachments;
using VisitzModel.Models.EntityTypes;
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

    public EntityType EntityType { get; private set; }

    public string RecordId { get; private set; }

    string getAttachmentsServiceId;
    string submitAttachmentsServiceId;
    RecordServiceInfo recordServiceInfo;

    string relativePath;

    string submittedAttachmentId;

    public CaseloadItem CaseloadItem { get; set; }

    string AttachmentName => attachmentDraft.Attachment.Filename;

    public AttachmentFiler AttachmentFiler { get; private set; }

    protected override async Task InitAsync()
	{
		await base.InitAsync();

        recordServiceInfo = new RecordServiceInfo(
                attachmentDraft.RelatedEntityType,
                CaseloadItem.RowId,
                attachmentDraft.Attachment.FileNumber,
                CaseloadItem.KeyPlayer.FirstName,
                CaseloadItem.KeyPlayer.LastName);

        getAttachmentsServiceId = GetAttachmentsService.MakeId(
            attachmentDraft.RelatedEntityType,
            CaseloadItem.RowId);

        submitAttachmentsServiceId = SubmitAttachmentService.MakeId(
            attachmentDraft.RelatedEntityType,
            CaseloadItem.RowId);

        WeakReferenceMessenger.Default.Register(this, submitAttachmentsServiceId);
        WeakReferenceMessenger.Default.Register(this, getAttachmentsServiceId);

        Wait(LocalizedStrings.LoginToSubmitAttachment);

        Publish();
    }

    bool disposed;
    protected override void Dispose(bool disposing)
    {
        if (!disposed && disposing)
        {
		    WeakReferenceMessenger.Default.UnregisterAll(this);

            disposed = true;
        }
        base.Dispose(disposing);
    }

    public async Task SetPayload(
        CaseloadItem item,
        AttachmentDraft draft,
        AttachmentFiler filer = null)
    {
        RecordId = item.RowId;
        EntityType = item.EntityType.ParseEntityType();
        attachmentDraft = draft;

        AttachmentFiler = filer ?? await VisitzFiles.GetAsync(
            item.EntityType.ParseEntityType(),
            item.CaseIncidentNumber,
            item.KeyPlayer.FirstName,
            item.KeyPlayer.LastName);
    }

    public override async void Publish()
	{
        AttachmentFormData submitEntity = await attachmentDraft.ToAttachmentFormData(AttachmentFiler);

        var startMessage = SubmitAttachmentService.MakeStartMessage(EntityType, RecordId, submitEntity);
        WeakReferenceMessenger.Default.Send(startMessage);
    }

    private void CallGetService()
    {
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
                relativePath = attachmentDraft.Attachment.RelativePath;
                submittedAttachmentId = message.ReturnPayload as string;
                await DiscardAttachmentDraft();
                CallGetService();
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

                using Realm realm = await VisitzRealms.GetIcmDataRealmAsync();
                var newAttachment = realm.Find<Attachment>(submittedAttachmentId);
                newAttachment.RelativePathBinding = relativePath;

                Complete();
            }
            else if (message.FinishedError)
            {
                RefreshError(LocalizedStrings.FailedToRefreshAttachments, message.Message);
                AttachmentFiler.DeleteFileFromDevice(relativePath);
            }
        }
    }

    async Task DiscardAttachmentDraft()
    {
        await attachmentDraft.Attachment.DeleteAsync(removeContent: false);
    }
}
