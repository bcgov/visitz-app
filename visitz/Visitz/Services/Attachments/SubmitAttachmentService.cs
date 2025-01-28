using Visitz.Services.Messages;
using VisitzApi;
using VisitzApi.Models.Attachments;

namespace Visitz.Services.Attachments;

internal class SubmitAttachmentService(Vpi vpi) : VisitzApiService(vpi)
{
    public static string MakeId(SubmitAttachmentEntity entity)
    {
        return MakeId(entity.EntityNumber, entity.AttachmentId);
    }

    public static string MakeId(string entityNumber, string attachmentId)
    {
        return $"{nameof(SubmitAttachmentService)}-{entityNumber}-{attachmentId}";
    }

    public static StartServiceMessage MakeStartMessage(SubmitAttachmentEntity submitEntity)
    {
        return new()
        {
            Payload = submitEntity,
            ServiceId = MakeId(submitEntity.EntityNumber, submitEntity.AttachmentId),
            ServiceType = typeof(SubmitAttachmentService),
        };
    }

    new SubmitAttachmentEntity Payload => (SubmitAttachmentEntity)base.Payload;

    public override string GetId()
    {
        return MakeId(Payload);
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
