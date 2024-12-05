using Oidc;
using Realms;
using Visitz.Services.Messages;
using Visitz.Storage;
using VisitzApi;
using VisitzModel.Models;

namespace Visitz.Services
{
    public class GetAllDataForOfflineService(Vpi vpi, ServiceHandler serviceHandler) : VisitzApiService(vpi)
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

        private ServiceHandler ServiceHandler { get; set; } = serviceHandler;

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
            var info = await OidcSessionInfo.GetAsync();
            await ServiceHandler.TryRunServiceAsync(GetCaseloadService.MakeStartMessage(info.Idir));
        }

        private async Task GetAllNotes()
        {
            using var realm = await VisitzRealms.GetIcmDataRealmAsync();

            var allIdEntities = realm
                .All<CaseloadItem>()
                .Freeze()
                .AsEnumerable()
                .Select(item => (item.CaseIncidentNumber, item.EntityType));

            var startMessage = GetNotesForRangeService.MakeStartMessage(allIdEntities);
            await ServiceHandler.TryRunServiceAsync(startMessage);
        }
    }
}
