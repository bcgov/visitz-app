using Visitz.Services.Base;
using Visitz.Services.Messages;
using Visitz.Storage;
using VisitzApi;
using VisitzModel.Extensions;
using VisitzModel.Extensions.EntityTypes;
using VisitzModel.Models.EntityTypes;
using VisitzModel.Models.Notes;
using VisitzModel.Storage;

namespace Visitz.Services.Notes;

#nullable enable

public class GetNotesService(Vpi vpi, LastUpdatedPrefs prefs) : VisitzApiService(vpi, prefs)
{
    public static string MakeId(string caseIncidentId)
    {
        return nameof(GetNotesService) + caseIncidentId;
    }

    public static StartServiceMessage MakeStartMessage(string caseIncidentId, EntityType entityType)
    {
        return MakeStartMessage((caseIncidentId, entityType));
    }

    public static StartServiceMessage MakeStartMessage(ValueTuple<string, EntityType> idEntityItem)
    {
        return new StartServiceMessage()
        {
            ServiceId = MakeId(idEntityItem.Item1),
            ServiceType = typeof(GetNotesService),
            Payload = idEntityItem,
        };
    }

    private ValueTuple<string, EntityType> PayloadTuple => (ValueTuple<string, EntityType>)Payload;

    public override string GetId()
    {
        var (caseIncidentId, _) = PayloadTuple;
        return MakeId(caseIncidentId);
    }

    protected override async Task RunApiServiceAsync()
    {
        await GetNotesAsync();
    }

    private async Task GetNotesAsync()
    {
        var (id, entityType) = PayloadTuple;

        string casedType = entityType.GetDisplayString().ToTitleCase();
        var notesFromApi = await Vpi.GetNotesAsync(id, casedType);

        var newNotes = NoteItem.FromApiEntities(id, entityType, notesFromApi);

        await VisitzRealms.EnqueueIcmDataActionAsync(async realm =>
            await NoteItem.UpsertNotesAsync(realm, id, entityType, newNotes)
        );

        ResultCode = Result.Successful;
    }
}
