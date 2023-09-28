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
            return MakeStartMessage((caseIncidentId, entityType));
        }

        public static StartServiceMessage MakeStartMessage(ValueTuple<string, string> idEntityItem)
        {
            return new StartServiceMessage()
            {
                ServiceId = MakeId(idEntityItem.Item1),
                ServiceType = typeof(GetNotesService),
                Payload = idEntityItem
            };
        }

        private ValueTuple<string, string> PayloadTuple => (ValueTuple<string, string>)Payload;

        public GetNotesService(Vpi vpi) : base(vpi) { }

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

            var notesFromApi = await Vpi.GetNotesAsync(id, entityType);
            var notes = NoteItem.FromApiEntities(id, notesFromApi);

            using var realm = await VisitzRealm.GetIcmDataAsync();
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

            ResultCode = Result.Successful;
        }
    }
}
