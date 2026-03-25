using Visitz.Services.Base;
using Visitz.Services.Messages;
using VisitzApi;
using VisitzApi.Models.Attachments;
using VisitzApi.Requests;
using VisitzModel.Models.EntityTypes;
using VisitzModel.Storage;

namespace Visitz.Services.Attachments;

internal class SubmitAttachmentService(Vpi vpi, LastUpdatedPrefs prefs) : VisitzApiService(vpi, prefs)
{
    public static string MakeId(EntityType type, string recordId)
    {
        return $"{nameof(SubmitAttachmentService)}-{type}-{recordId}";
    }

    public static StartServiceMessage MakeStartMessage(EntityType type, string recordId, AttachmentFormData data)
    {
        return new()
        {
            Payload = (type, recordId, data),
            ServiceId = MakeId(type, recordId),
            ServiceType = typeof(SubmitAttachmentService),
        };
    }

    new (EntityType Type, string RecordId, AttachmentFormData) Payload =>
        ((EntityType, string, AttachmentFormData))base.Payload;

    public override string GetId()
    {
        return MakeId(Payload.Type, Payload.RecordId);
    }

    protected override async Task RunApiServiceAsync()
    {
        await SubmitAttachmentAsync();
    }

    async Task SubmitAttachmentAsync()
    {
        var (type, id, data) = Payload;

        var (result, attachmentId) = await Vpi.SubmitAttachmentAsync((ApiRecordType)type, id, data);

        ResultCode = result ? Result.Successful : Result.Error;
        ReturnPayload = attachmentId;

        data.Dispose();
    }
}
