using Oidc;
using Realms;
using Visitz.Services.Base;
using Visitz.Services.Messages;
using Visitz.Services.Notes;
using Visitz.Services.People;
using Visitz.Services.Visits;
using Visitz.Storage;
using VisitzApi;
using VisitzModel.Models;
using VisitzModel.Models.Caseload;
using VisitzModel.Storage;

namespace Visitz.Services.Caseload
{
    public class GetAllDataForOfflineService(Vpi vpi, ServiceHandler serviceHandler, LastUpdatedPrefs prefs)
        : VisitzApiService(vpi, prefs)
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
                await MultiGetSubData();
            });

            ResultCode = Result.Successful;
        }

        private async Task GetCaseload()
        {
            var info = await OidcSessionInfo.GetAsync();
            await ServiceHandler.TryRunServiceAsync(GetCaseloadService.MakeStartMessage(info.Idir));
        }

        private async Task MultiGetSubData()
        {
            using var realm = await VisitzRealms.GetIcmDataRealmAsync();

            var cases = realm
                .All<CaseRecord>()
                .Freeze()
                .AsEnumerable()
                .Select(@case => new RecordServiceInfo(@case));

            var incidents = realm
                .All<IncidentRecord>()
                .Freeze()
                .AsEnumerable()
                .Select(incident => new RecordServiceInfo(incident));

            // TODO: Memos, SRs

            await Task.WhenAll(
                GetAllNotes(realm),
                GetAllVisits(realm),
                GetAllContacts(cases, incidents),
                GetAllSupportNetworkItems(cases, incidents)
            );
        }

        private async Task GetAllNotes(Realm realm)
        {
            var allIdEntities = realm
                .All<CaseloadItem>()
                .Freeze()
                .AsEnumerable()
                .Select(item => (item.CaseIncidentNumber, item.EntityType));

            var startMessage = GetNotesForRangeService.MakeStartMessage(allIdEntities);
            await ServiceHandler.TryRunServiceAsync(startMessage);
        }

        private async Task GetAllVisits(Realm realm)
        {
            var allCaseIds = realm
                .All<CaseRecord>()
                .Freeze()
                .AsEnumerable()
                .Select(@case => @case.Id);

            var startMessage = GetVisitsByRangeService.MakeStartMessage(allCaseIds);
            await ServiceHandler.TryRunServiceAsync(startMessage);
        }

        private async Task GetAllContacts(
            IEnumerable<RecordServiceInfo> cases,
            IEnumerable<RecordServiceInfo> incidents)
        {
            var all = cases.Concat(incidents);

            var startMessage = GetContactsByRangeService.MakeStartMessage(all);
            await ServiceHandler.TryRunServiceAsync(startMessage);
        }

        private async Task GetAllSupportNetworkItems(
            IEnumerable<RecordServiceInfo> cases,
            IEnumerable<RecordServiceInfo> incidents)
        {
            var all = cases.Concat(incidents);

            var startMessage = GetSupportNetworkByRangeService.MakeStartMessage(all);
            await ServiceHandler.TryRunServiceAsync(startMessage);
        }
    }
}
