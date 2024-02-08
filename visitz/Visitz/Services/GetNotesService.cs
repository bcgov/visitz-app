using Visitz.Services.Messages;
using Visitz.Storage;
using VisitzApi;
using VisitzModel.Models;

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
            var newNotes = NoteItem.FromApiEntities(id, notesFromApi);

            using var realm = await VisitzRealm.GetIcmDataAsync();
            var currentNotes = NoteItem.GetNotesByEntityId(realm, id);
            var deletedNotes = currentNotes.ExceptBy(newNotes.Select(NoteSelector), NoteSelector);

            await realm.WriteAsync(() =>
            {
                foreach (var deletedNote in deletedNotes)
                    realm.Remove(deletedNote);

                realm.Add(newNotes, update: true);
            });

            ResultCode = Result.Successful;
        }

        static string NoteSelector(NoteItem note) => note.FullID;
    }
}
