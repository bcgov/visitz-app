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
            var allNotesServiceTasks = IdEntityItems.Select(item => 
                ServiceHandler.TryRunServiceAsync(GetNotesService.MakeStartMessage(item)));

            await Task.WhenAll(allNotesServiceTasks);

            ResultCode = Result.Successful;
        }
    }
}
