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

internal partial class AttachmentDraftPublishViewModel : PublishViewModel, IRecipient<ServiceStateMessage>
{
    AttachmentDraft? attachmentDraft;

    public AttachmentDraft AttachmentDraft
    {
        set
        {
            attachmentDraft = value;

            Title = AttachmentName;
        }
    }

    public EntityType EntityType { get; private set; }

    public string RecordId { get; private set; } = string.Empty;

    string getAttachmentsServiceId = string.Empty;
    string submitAttachmentsServiceId = string.Empty;
    RecordServiceInfo? recordServiceInfo;

    string relativePath = string.Empty;

    string? submittedAttachmentId;

    string AttachmentName => attachmentDraft?.Attachment?.Filename ?? string.Empty;

    public AttachmentFiler? AttachmentFiler { get; private set; }

    public AttachmentFormData? AttachmentToSubmit { get; private set; }

    public async Task SetPayload(IBusinessObject item, AttachmentDraft draft, AttachmentFiler? filer = null)
    {
        ArgumentNullException.ThrowIfNull(draft.Attachment);

        RecordId = item.Id;
        EntityType = item.EntityType;
        attachmentDraft = draft;

        AttachmentFiler = filer ?? await VisitzFiles.GetAsync(item);
        var keyPlayer = item.GetKeyPlayer();

        ArgumentNullException.ThrowIfNull(keyPlayer);

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

        ArgumentNullException.ThrowIfNull(attachmentDraft);
        ArgumentNullException.ThrowIfNull(AttachmentFiler);

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
        ArgumentNullException.ThrowIfNull(AttachmentToSubmit);

        var startMessage = SubmitAttachmentService.MakeStartMessage(EntityType, RecordId, AttachmentToSubmit);

        WeakReferenceMessenger.Default.Send(startMessage);
    }

    private void CallGetService()
    {
        ArgumentNullException.ThrowIfNull(recordServiceInfo);

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

                relativePath = attachmentDraft?.Attachment?.RelativePath ?? string.Empty;
                submittedAttachmentId = message.ReturnPayload as string;

                await MoveAttachmentToIcmDataRealm(submittedAttachmentId);
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
                Complete();
            }
            else if (message.FinishedError)
            {
                RefreshError(LocalizedStrings.FailedToRefreshAttachments, message.Message);
                AttachmentFiler.DeleteFileFromDevice(relativePath);
            }
        }
    }

    async Task MoveAttachmentToIcmDataRealm(string? newDatabaseId)
    {
        if (newDatabaseId == null || attachmentDraft == null || attachmentDraft.Attachment == null)
            return;

        using Realm realm = await VisitzRealms.GetIcmDataRealmAsync();

        Attachment attachment = new() { Id = newDatabaseId, RelativePath = attachmentDraft.Attachment.RelativePath };
        attachment.CopyFrom(attachmentDraft.Attachment);

        await attachmentDraft.DeleteAsync(deleteAttachment: true, deleteAttachmentFile: false);
        await realm.CommitAsync(() => realm.Add(attachment));
    }
}
