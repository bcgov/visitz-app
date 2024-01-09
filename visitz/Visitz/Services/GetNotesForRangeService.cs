using Visitz.Services.Messages;
using VisitzApi;

namespace Visitz.Services
{
    public class GetNotesForRangeService : VisitzApiService
    {
        public static string MakeId()
        {
            return nameof(GetNotesForRangeService);
        }

        public static StartServiceMessage MakeStartMessage(IEnumerable<ValueTuple<string, string>> idEntityItems)
        {
            return new StartServiceMessage()
            {
                ServiceId = MakeId(),
                ServiceType = typeof(GetNotesForRangeService),
                Payload = idEntityItems,
            };
        }

        private ServiceHandler ServiceHandler { get; set; }

        private IEnumerable<ValueTuple<string,string>> IdEntityItems => 
            (IEnumerable<ValueTuple<string, string>>)Payload;

        public GetNotesForRangeService(Vpi vpi, ServiceHandler serviceHandler) : base(vpi) 
        {
            ServiceHandler = serviceHandler;
        }

        public override string GetId()
        {
            return MakeId();
        }

        protected override async Task RunApiServiceAsync()
        {
            await GetAllNotesAsync();
        }

        private async Task GetAllNotesAsync()
        {
            /*
             * TODO: Improve efficiency of this operation.
             * 
             * Previously, GetNotesService was run concurrently but after crashing issues it has been converted to run
             * sequentially instead.
             * 
             * Maybe run network requests concurrently, then Realm I/O sequentially as Tasks complete?
             */
            foreach (var (id, entityType) in IdEntityItems)
                await ServiceHandler.TryRunServiceAsync(GetNotesService.MakeStartMessage(id, entityType));
        }
    }
}
