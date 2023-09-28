using Visitz.Authentication.Keycloak;
using Visitz.Models;
using Visitz.Services.Messages;
using Visitz.Storage;
using VisitzApi;

namespace Visitz.Services
{
    public class GetAllDataForOfflineService : VisitzApiService
    {
        public static string MakeId()
        {
            return nameof(GetAllDataForOfflineService);
        }

        public static StartServiceMessage MakeStartMessage()
        {
            return new StartServiceMessage()
            {
                ServiceId = MakeId(),
                ServiceType = typeof(GetAllDataForOfflineService),
                Payload = null,
            };
        }

        private ServiceHandler ServiceHandler { get; set; }

        public GetAllDataForOfflineService(Vpi vpi, ServiceHandler serviceHandler) : base(vpi)
        {
            ServiceHandler = serviceHandler;
        }

        public override string GetId()
        {
            return MakeId();
        }

        protected override async Task RunApiServiceAsync()
        {
            await Task.Run(async () =>
            {
                await GetCaseload();
                await GetAllNotes();
            });

            ResultCode = Result.Successful;
        }

        private async Task GetCaseload()
        {
            var info = await VisitzSessionInfo.GetAsync();
            await ServiceHandler.TryRunServiceAsync(GetCaseloadService.MakeStartMessage(info.Idir));
        }

        private async Task GetAllNotes()
        {
            using var realm = await VisitzRealm.GetIcmDataAsync();

            var allIdEntities = realm
                .All<CaseloadItem>()
                .ToList()
                .Select(item => (item.CaseIncidentNumber, item.EntityType));

            var startMessage = GetNotesForRangeService.MakeStartMessage(allIdEntities);
            await ServiceHandler.TryRunServiceAsync(startMessage);
        }
    }
}
