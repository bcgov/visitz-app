using Visitz.Models;
using Visitz.Services.Messages;
using Visitz.Storage;
using VisitzApi;

namespace Visitz.Services
{
    public class GetNotesService : VisitzApiService
    {
        public static string MakeId(string caseIncidentId)
        {
            return nameof(GetNotesService) + caseIncidentId;
        }

        public static StartServiceMessage MakeStartMessage(string caseIncidentId, string entityType)
        {
            return new StartServiceMessage()
            {
                ServiceId = MakeId(caseIncidentId),
                ServiceType = typeof(GetNotesService),
                Payload = (caseIncidentId, entityType)
            };
        }

        private ValueTuple<string, string> PayloadTuple => (ValueTuple<string, string>)Payload;

        public GetNotesService(Vpi vpi) : base(vpi) { }

        public override string GetId()
        {
            var (caseIncidentId, _) = PayloadTuple;
            return MakeId(caseIncidentId);
        }

        protected override async Task RunAsync()
        {
            await GetNotesAsync();
        }

        private async Task GetNotesAsync()
        {
            var (id, entityType) = PayloadTuple;

            var notesFromApi = await Vpi.GetNotesAsync(id, entityType);
            var notes = NoteItem.FromApiEntities(id, notesFromApi);

            using var realm = await IcmDataRealm.GetAsync();
            await realm.WriteAsync(() =>
            {
                var allNotesByEntityId = realm
                    .All<NoteItem>()
                    .Where(note => note.IcmId == id);

                // For this ICM entity, remove all local notes from storage to
                // automatically handle if notes were deleted in ICM.
                realm.RemoveRange(allNotesByEntityId);

                realm.Add(notes);
            });
        }
    }
}
