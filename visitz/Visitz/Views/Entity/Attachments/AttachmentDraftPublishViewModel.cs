using CommunityToolkit.Mvvm.Messaging;
using Realms;
using Visitz.Resources.Localization;
using Visitz.Services;
using Visitz.Services.Attachments;
using Visitz.Services.Base;
using Visitz.Services.Messages;
using Visitz.Storage;
using Visitz.Views.BaseClasses.Publishing;
using VisitzApi.Models.Attachments;
using VisitzModel.Extensions;
using VisitzModel.Models.Attachments;
using VisitzModel.Models.Caseload;
using VisitzModel.Models.EntityTypes;
using VisitzModel.Storage.Filesystem;

namespace Visitz.Views.Entity.Attachments;

internal class AttachmentDraftPublishViewModel : PublishViewModel, IRecipient<ServiceStateMessage>
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

    string AttachmentName => attachmentDraft.Attachment.Filename;

    public AttachmentFiler AttachmentFiler { get; private set; }

    public AttachmentFormData AttachmentToSubmit { get; private set; }

    public async Task SetPayload(IBusinessObject item, AttachmentDraft draft, AttachmentFiler filer = null)
    {
        RecordId = item.Id;
        EntityType = item.EntityType;
        attachmentDraft = draft;

        AttachmentFiler = filer ?? await VisitzFiles.GetAsync(item);
        var keyPlayer = item.GetKeyPlayer();

        recordServiceInfo = new RecordServiceInfo(
            attachmentDraft.RelatedEntityType,
            attachmentDraft.RelatedEntitySubtype,
            RecordId,
            attachmentDraft.Attachment.FileNumber,
            keyPlayer.FirstName,
            keyPlayer.LastName
        );
    }

    protected override async Task InitAsync()
    {
        await base.InitAsync();

        getAttachmentsServiceId = GetAttachmentsService.MakeId(attachmentDraft.RelatedEntityType, RecordId);

        submitAttachmentsServiceId = SubmitAttachmentService.MakeId(attachmentDraft.RelatedEntityType, RecordId);

        WeakReferenceMessenger.Default.Register(this, submitAttachmentsServiceId);
        WeakReferenceMessenger.Default.Register(this, getAttachmentsServiceId);

        AttachmentToSubmit = await attachmentDraft.ToAttachmentFormData(AttachmentFiler);

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

    public override void Publish()
    {
        var startMessage = SubmitAttachmentService.MakeStartMessage(EntityType, RecordId, AttachmentToSubmit);

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

                if (realm.Find<Attachment>(submittedAttachmentId) is Attachment newAttachment)
                    // TODO: sometimes we can't find the new attachment.
                    // Need to look into this, but since the user can just
                    // refresh their caseload normally it's not the highest
                    // priority.
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
