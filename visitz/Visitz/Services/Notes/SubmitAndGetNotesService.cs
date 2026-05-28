using Visitz.Services.Base;
using Visitz.Services.Messages;
using VisitzApi;
using VisitzApi.Models;
using VisitzModel.Extensions.EntityTypes;
using VisitzModel.Storage;

namespace Visitz.Services.Notes;

#nullable enable

public class SubmitAndGetNotesService(Vpi vpi, ServiceHandler serviceHandler, LastUpdatedPrefs prefs)
    : VisitzApiService(vpi, prefs)
{
    public static string MakeId(string entityNumber, string notePeriod)
    {
        return $"{nameof(SubmitAndGetNotesService)}-{entityNumber}-{notePeriod}";
    }

    public static StartServiceMessage MakeStartMessage(SubmitNoteEntity submitEntity)
    {
        return new StartServiceMessage()
        {
            ServiceId = MakeId(submitEntity.EntityNumber, submitEntity.NotePeriod),
            ServiceType = typeof(SubmitAndGetNotesService),
            Payload = submitEntity,
        };
    }

    private new SubmitNoteEntity Payload => (SubmitNoteEntity)base.Payload;

    private ServiceHandler ServiceHandler { get; set; } = serviceHandler;

    public override string GetId()
    {
        return MakeId(Payload.EntityNumber, Payload.NotePeriod);
    }

    protected override async Task RunApiServiceAsync()
    {
        if (await SubmitNote(Payload) && await GetNotes(Payload.EntityNumber, Payload.EntityType))
            ResultCode = Result.Successful;
    }

    private async Task<bool> SubmitNote(SubmitNoteEntity noteEntity)
    {
        var result = await ServiceHandler.TryRunServiceAsync(SubmitNoteService.MakeStartMessage(noteEntity));
        return result == Result.Successful;
    }

    private async Task<bool> GetNotes(string caseIncidentId, string entityType)
    {
        var startMessage = GetNotesService.MakeStartMessage(caseIncidentId, entityType.ParseEntityType());
        var result = await ServiceHandler.TryRunServiceAsync(startMessage);
        return result == Result.Successful;
    }
}
