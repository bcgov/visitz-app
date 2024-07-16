using Visitz.Services.Messages;
using VisitzApi;
using VisitzApi.Models.Attachments;

namespace Visitz.Services;

internal class SubmitAttachmentService(Vpi vpi) : VisitzApiService(vpi)
{
	public static string MakeId(string entityNumber, string attachmentName)
	{
		return $"{nameof(SubmitAttachmentService)}-{entityNumber}-{attachmentName}";
	}

	public static StartServiceMessage MakeStartMessage(SubmitAttachmentEntity submitEntity)
	{
		return new()
		{
			Payload = submitEntity,
			ServiceId = MakeId(submitEntity.EntityNumber, submitEntity.FileName),
			ServiceType = typeof(SubmitAttachmentService),
		};
	}

	new SubmitAttachmentEntity Payload => (SubmitAttachmentEntity)base.Payload;

	public override string GetId()
	{
		return MakeId(Payload.EntityNumber, Payload.FileName);
	}

	protected override async Task RunApiServiceAsync()
	{
		await SubmitAttachmentAsync();
	}

	async Task SubmitAttachmentAsync()
	{
		var (status, _) = await Vpi.SubmitAttachmentAsync(Payload);

		ResultCode = status ? Result.Successful : Result.Error;
	}
}
