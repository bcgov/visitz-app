using Oidc;
using Realms;
using Visitz.Services.Attachments;
using Visitz.Services.Base;
using Visitz.Services.Messages;
using Visitz.Services.Notes;
using Visitz.Services.People;
using Visitz.Services.SafetyAssessments;
using Visitz.Services.Visits;
using Visitz.Storage;
using VisitzApi;
using VisitzModel.Models;
using VisitzModel.Models.Caseload;
using VisitzModel.Models.EntityTypes;
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

        public static StartServiceMessage MakeStartMessage(bool forceDownload = false)
        {
            return new StartServiceMessage()
            {
                ServiceId = MakeId(),
                ServiceType = typeof(GetAllDataForOfflineService),
                Payload = forceDownload,
            };
        }

        private ServiceHandler ServiceHandler { get; set; } = serviceHandler;

        private bool ShouldForceDownload => (bool)Payload;

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

            var caseloadMessage = GetCaseloadService.MakeStartMessage(info.Idir, ShouldForceDownload);
            await ServiceHandler.TryRunServiceAsync(caseloadMessage);
        }

        private async Task MultiGetSubData()
        {
            using var realm = await VisitzRealms.GetIcmDataRealmAsync();

            var cases = realm.All<CaseRecord>().Freeze().AsEnumerable()
                .Select(@case => new RecordServiceInfo(@case));

            var incidents = realm.All<IncidentRecord>().Freeze().AsEnumerable()
                .Select(incident => new RecordServiceInfo(incident));

            var memos = realm.All<MemoRecord>().Freeze().AsEnumerable()
                .Select(memo => new RecordServiceInfo(memo));

            var srs = realm.All<ServiceRequestRecord>().Freeze().AsEnumerable()
                .Select(sr => new RecordServiceInfo(sr));

            await Task.WhenAll(
                GetAllNotes(realm),
                GetAllVisits(realm),
                GetAllContacts(cases, incidents, memos, srs),
                GetAllSupportNetworkItems(cases, incidents, srs),
                GetAllAttachments(cases, incidents, memos, srs),
                GetPartialAttachments(cases, incidents, memos, srs),
                GetAllSafetyAssessments(incidents)
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
                .Where(@case => @case.Type == EntitySubtype.ChildServices)
                .Select(@case => @case.Id);

            var startMessage = GetVisitsByRangeService.MakeStartMessage(allCaseIds);
            await ServiceHandler.TryRunServiceAsync(startMessage);
        }

        private async Task GetAllContacts(
            IEnumerable<RecordServiceInfo> cases,
            IEnumerable<RecordServiceInfo> incidents,
            IEnumerable<RecordServiceInfo> memos,
            IEnumerable<RecordServiceInfo> srs)
        {
            var all = cases.Concat(incidents).Concat(memos).Concat(srs);

            var startMessage = GetContactsByRangeService.MakeStartMessage(all);
            await ServiceHandler.TryRunServiceAsync(startMessage);
        }

        private async Task GetAllSupportNetworkItems(
            IEnumerable<RecordServiceInfo> cases,
            IEnumerable<RecordServiceInfo> incidents,
            IEnumerable<RecordServiceInfo> srs)
        {
            var all = cases.Concat(incidents).Concat(srs);

            var startMessage = GetSupportNetworkByRangeService.MakeStartMessage(all);
            await ServiceHandler.TryRunServiceAsync(startMessage);
        }

        private async Task GetAllAttachments(
            IEnumerable<RecordServiceInfo> cases,
            IEnumerable<RecordServiceInfo> incidents,
            IEnumerable<RecordServiceInfo> memos,
            IEnumerable<RecordServiceInfo> srs)
        {
            var all = cases.Concat(incidents).Concat(memos).Concat(srs);

            var startMessage = GetAttachmentsByRangeService.MakeStartMessage(all);
            await ServiceHandler.TryRunServiceAsync(startMessage);
        }

        private async Task GetPartialAttachments(
            IEnumerable<RecordServiceInfo> cases,
            IEnumerable<RecordServiceInfo> incidents,
            IEnumerable<RecordServiceInfo> memos,
            IEnumerable<RecordServiceInfo> srs)
        {
            var all = cases.Concat(incidents).Concat(memos).Concat(srs);

            var startMessage = GetPartialAttachmentsByRangeDownloadService.MakeStartMessage(all);
            await ServiceHandler.TryRunServiceAsync(startMessage);
        }

        private async Task GetAllSafetyAssessments(IEnumerable<RecordServiceInfo> incidents)
        {
            var startMessage = GetSafetyAssessmentsByRangeService.MakeStartMessage(incidents);
            await ServiceHandler.TryRunServiceAsync(startMessage);
        }
    }
}
