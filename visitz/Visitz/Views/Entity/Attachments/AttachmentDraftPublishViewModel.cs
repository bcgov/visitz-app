using CommunityToolkit.Mvvm.Messaging;
using Visitz.Documents;
using Visitz.Resources.Localization;
using Visitz.Services;
using Visitz.Views.BaseClasses.Publishing;
using VisitzApi.Models.Attachments;
using VisitzModel.Extensions;
using VisitzModel.Models;
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

	SubmitAttachmentEntity submitEntity;

	string AttachmentName => attachmentDraft.Attachment.Filename;

	public AttachmentFiler AttachmentFiler { get; set; }

	public override async void Create()
	{
		base.Create();

		var converter = TryMakeImageToPdfConverter(attachmentDraft);
		submitEntity = await attachmentDraft.ToSubmitAttachmentEntity(AttachmentFiler, converter);
		
		WeakReferenceMessenger.Default.Register(this, SubmitAttachmentService.MakeId(submitEntity));

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

	public async void Receive(ServiceStateMessage message)
	{
		if (message.Status == VisitzService.State.Running)
			Publishing(LocalizedStrings.PublishingAttachmentToIcm.Format(AttachmentName));
		else if (message.FinishedSuccess)
		{
			Published(LocalizedStrings.AttachmentPublishSuccess.Format(AttachmentName));
			await DiscardAttachmentDraft();
			Complete();
		}
		else if (message.FinishedCancelled)
			Cancel(LocalizedStrings.LoginToSubmitAttachment);
		else if (message.FinishedError)
			PublishError(LocalizedStrings.FailedToPublishToIcm, message.Message);
	}

	async Task DiscardAttachmentDraft()
	{
		await attachmentDraft.Attachment.DeleteAsync();
	}

	static ImagePdfStreamConverter TryMakeImageToPdfConverter(AttachmentDraft attachmentDraft)
	{
		return Attachment.AllowedImageTypes.Contains(attachmentDraft.Attachment.Extension)
			? new ImagePdfStreamConverter(attachmentDraft.Attachment.Filename)
			: null;
	}
}
