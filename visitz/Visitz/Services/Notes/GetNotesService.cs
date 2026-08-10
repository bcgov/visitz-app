using System.Collections.Concurrent;
using Visitz.Services.Base;
using Visitz.Services.Messages;
using Visitz.Storage;
using VisitzApi;
using VisitzApi.Requests;
using VisitzModel.Extensions;
using VisitzModel.Models.EntityTypes;
using VisitzModel.Models.Notes;
using VisitzModel.Storage;

namespace Visitz.Services.Notes;

internal class GetNotesService(Vpi vpi, LastUpdatedPrefs prefs) : ApiPaginationService(vpi, prefs)
{
    readonly ConcurrentBag<NoteItem> Notes = [];

    public static string MakeId(RecordServiceInfo parentInfo)
    {
        return $"{nameof(GetNotesService)}-{parentInfo.Type}-{parentInfo.Id}";
    }

    public static StartServiceMessage MakeStartMessage(RecordServiceInfo parentInfo)
    {
        return new StartServiceMessage()
        {
            ServiceId = MakeId(parentInfo),
            ServiceType = typeof(GetNotesService),
            Payload = parentInfo,
        };
    }

    private RecordServiceInfo ParentInfo => (RecordServiceInfo)Payload;

    public override string GetId()
    {
        return MakeId(ParentInfo);
    }

    protected override async Task<int> RunPageInParallelAsync(Pagination pagination)
    {
        int total;

        if (ParentInfo.Type == EntityType.Case)
        {
            (total, var notesFromApi) = await Vpi.GetCaseNotesAsync(ParentInfo.Id, pagination: pagination);
            Notes.AddAll(NoteItem.FromApiEntities(ParentInfo.Id, notesFromApi));
        }
        else if (ParentInfo.Type is EntityType.Incident or EntityType.ServiceRequest)
        {
            (total, var notesFromApi) = await Vpi.GetResponseNarrativesAsync(
                (ApiRecordType)ParentInfo.Type,
                ParentInfo.Id,
                pagination
            );
            Notes.AddAll(NoteItem.FromApiEntities(ParentInfo.Type, ParentInfo.Id, notesFromApi));
        }
        else
            throw new InvalidOperationException($"Type '{ParentInfo.Type}' not allowed for notes");

        return total;
    }

    protected override async Task AfterRun()
    {
        await base.AfterRun();

        await VisitzRealms.EnqueueIcmDataActionAsync(async realm =>
            await NoteItem.SynchronizeAsync(realm, ParentInfo.Id, ParentInfo.Type, Notes)
        );

        ResultCode = Result.Successful;
    }
}
