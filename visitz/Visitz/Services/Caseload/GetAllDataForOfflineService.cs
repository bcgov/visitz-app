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
using VisitzModel.Models.Caseload;
using VisitzModel.Models.EntityTypes;
using VisitzModel.Storage;

namespace Visitz.Services.Caseload
{
    public class GetAllDataForOfflineService(
        Vpi vpi,
        ServiceHandler serviceHandler,
        LastUpdatedPrefs prefs)
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
            var caseloadMessage = GetCaseloadService.MakeStartMessage(ShouldForceDownload);
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

            var casesIncidentsSrs = cases.Concat(incidents).Concat(srs);
            var all = casesIncidentsSrs.Concat(memos);

            List<Exception> exceptions = [];

            await Task.WhenAll(
                GetAllNotes(casesIncidentsSrs, exceptions),
                GetAllVisits(realm, exceptions),
                GetAllContacts(all, exceptions),
                GetAllSupportNetworkItems(casesIncidentsSrs, exceptions),
                GetAllAttachments(all, exceptions),
                GetAllSafetyAssessments(incidents, exceptions)
            );

            if (exceptions.Count > 1)
                throw new AggregateException(exceptions);
            else if (exceptions.Count > 0)
                throw exceptions.First();
        }

        private async Task GetAllNotes(
            IEnumerable<RecordServiceInfo> casesIncidentsSrs,
            List<Exception> exceptions)
        {
            try
            {
                var allIdEntities = casesIncidentsSrs
                    .Select(item => (item.FileNumber, item.Type));

                var startMessage = GetNotesForRangeService.MakeStartMessage(allIdEntities);
                await ServiceHandler.TryRunServiceAsync(startMessage);
            }
            catch (Exception ex)
            {
                exceptions.Add(ex);
            }
        }

        private async Task GetAllVisits(Realm realm, List<Exception> exceptions)
        {
            try
            {
                var allCaseIds = realm
                    .All<CaseRecord>()
                    .Freeze()
                    .AsEnumerable()
                    .Where(@case => @case.EntitySubtype == EntitySubtype.ChildServices)
                    .Select(@case => @case.Id);

                var startMessage = GetVisitsByRangeService.MakeStartMessage(allCaseIds);
                await ServiceHandler.TryRunServiceAsync(startMessage);
            }
            catch (Exception ex)
            {
                exceptions.Add(ex);
            }
        }

        private async Task GetAllContacts(
            IEnumerable<RecordServiceInfo> all,
            List<Exception> exceptions)
        {
            try
            {
                var startMessage = GetContactsByRangeService.MakeStartMessage(all);
                await ServiceHandler.TryRunServiceAsync(startMessage);
            }
            catch (Exception ex)
            {
                exceptions.Add(ex);
            }
        }

        private async Task GetAllSupportNetworkItems(
            IEnumerable<RecordServiceInfo> casesIncidentsSrs,
            List<Exception> exceptions)
        {
            try
            {
                var startMessage = GetSupportNetworkByRangeService.MakeStartMessage(casesIncidentsSrs);
                await ServiceHandler.TryRunServiceAsync(startMessage);
            }
            catch (Exception ex)
            {
                exceptions.Add(ex);
            }
        }

        private async Task GetAllAttachments(
            IEnumerable<RecordServiceInfo> all,
            List<Exception> exceptions)
        {
            try
            {
                var startMessage = GetAttachmentsByRangeService.MakeStartMessage(all);
                await ServiceHandler.TryRunServiceAsync(startMessage);
            }
            catch (Exception ex)
            {
                exceptions.Add(new Exception("An error occurred when trying to download attachment metadata", ex));
            }

            await GetPartialAttachments(all, exceptions);
        }

        private async Task GetPartialAttachments(
            IEnumerable<RecordServiceInfo> all,
            List<Exception> exceptions)
        {
            try
            {
                var startMessage = GetPartialAttachmentsByRangeDownloadService.MakeStartMessage(all);
                await ServiceHandler.TryRunServiceAsync(startMessage);
            }
            catch (Exception ex)
            {
                exceptions.Add(ex);
            }
        }

        private async Task GetAllSafetyAssessments(
            IEnumerable<RecordServiceInfo> incidents,
            List<Exception> exceptions)
        {
            try
            {
                var startMessage = GetSafetyAssessmentsByRangeService.MakeStartMessage(incidents);
                await ServiceHandler.TryRunServiceAsync(startMessage);
            }
            catch (Exception ex)
            {
                exceptions.Add(ex);
            }
        }
    }
}
