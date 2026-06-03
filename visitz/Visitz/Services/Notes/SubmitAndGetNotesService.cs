using Visitz.Services.Base;
using Visitz.Services.Messages;
using VisitzApi;
using VisitzApi.Models.Notes;
using VisitzModel.Storage;

namespace Visitz.Services.Notes;

#nullable enable

internal class SubmitAndGetNotesService(Vpi vpi, ServiceHandler serviceHandler, LastUpdatedPrefs prefs)
    : VisitzApiService(vpi, prefs)
{
    public static string MakeId(string parentId, string notePeriod)
    {
        return $"{nameof(SubmitAndGetNotesService)}-{parentId}-{notePeriod}";
    }

    public static StartServiceMessage MakeStartMessage(SubmitNoteEntity submitEntity, RecordServiceInfo info)
    {
        return new StartServiceMessage()
        {
            ServiceId = MakeId(submitEntity.EntityNumber, submitEntity.NotePeriod),
            ServiceType = typeof(SubmitAndGetNotesService),
            Payload = (submitEntity, info),
        };
    }

    private new (SubmitNoteEntity, RecordServiceInfo) Payload => ((SubmitNoteEntity, RecordServiceInfo))base.Payload;

    SubmitNoteEntity SubmitEntity => Payload.Item1;

    RecordServiceInfo ParentInfo => Payload.Item2;

    private ServiceHandler ServiceHandler { get; set; } = serviceHandler;

    public override string GetId()
    {
        return MakeId(SubmitEntity.EntityNumber, SubmitEntity.NotePeriod);
    }

    protected override async Task RunApiServiceAsync()
    {
        if (await SubmitNote(SubmitEntity) && await GetNotes())
            ResultCode = Result.Successful;
    }

    private async Task<bool> SubmitNote(SubmitNoteEntity noteEntity)
    {
        var result = await ServiceHandler.TryRunServiceAsync(SubmitNoteService.MakeStartMessage(noteEntity));
        return result == Result.Successful;
    }

    private async Task<bool> GetNotes()
    {
        var startMessage = GetNotesService.MakeStartMessage(ParentInfo);
        var result = await ServiceHandler.TryRunServiceAsync(startMessage);
        return result == Result.Successful;
    }
}
