using System.Net;
using Visitz.Services.Base;
using Visitz.Services.Messages;
using Visitz.Storage;
using VisitzApi;
using VisitzApi.Models.Base;
using VisitzApi.Models.Caseload;
using VisitzModel.Extensions.EntityTypes;
using VisitzModel.Models;
using VisitzModel.Models.Caseload;
using VisitzModel.Models.EntityTypes;
using VisitzModel.Storage;

namespace Visitz.Services.Caseload
{
    public class GetCaseloadService(Vpi vpi, LastUpdatedPrefs prefs) : VisitzApiService(vpi, prefs)
    {
        public static string MakeId()
        {
            return nameof(GetCaseloadService);
        }

        public static StartServiceMessage MakeStartMessage(string idir, bool forceDownload)
        {
            return new StartServiceMessage
            {
                ServiceId = MakeId(),
                ServiceType = typeof(GetCaseloadService),
                Payload = (idir, forceDownload),
            };
        }

        public new (string Idir, bool Force) Payload => ((string, bool))base.Payload;

        protected override async Task RunApiServiceAsync()
        {
            await GetCaseloadV1Async();
            await DownloadAndSaveCaseloadV2Async();

            ResultCode = Result.Successful;
        }

        private async Task GetCaseloadV1Async()
        {
            var caseloadFromApi = await Vpi.GetCaseloadV1Async(Payload.Idir);
            var caseloadContent = CaseloadItem.FromApiEntities(caseloadFromApi);

            caseloadContent = FilterNonCasesAndIncidents(caseloadContent);

            using var realm = await VisitzRealms.GetIcmDataRealmAsync();
            await CaseloadItem.ReplaceCaseloadWithAsync(realm, caseloadContent);
        }

        private async Task DownloadAndSaveCaseloadV2Async()
        {
            DateTimeOffset? after = Payload.Force ? null : (DateTimeOffset?)LastUpdatedPrefs.Get(GetId());

            CaseloadJson caseloadFromApi = await Vpi.GetCaseloadV2Async(after: after);

            using var realm = await VisitzRealms.GetIcmDataRealmAsync();

            List<InvalidOperationException> invalidOps = [];

            if (CanSynchronize(caseloadFromApi.Cases, invalidOps))
                await CaseRecord.SynchronizeCasesAsync(realm, caseloadFromApi.Cases);

            if (CanSynchronize(caseloadFromApi.Incidents, invalidOps))
                await IncidentRecord.SynchronizeAsync(realm, caseloadFromApi.Incidents);

            if (CanSynchronize(caseloadFromApi.Memos, invalidOps))
                await MemoRecord.SynchronizeAsync(realm, caseloadFromApi.Memos);

            if (CanSynchronize(caseloadFromApi.ServiceRequests, invalidOps))
                await ServiceRequestRecord.SynchronizeAsync(realm, caseloadFromApi.ServiceRequests);

            if (invalidOps.Count > 0)
                throw new AggregateException(invalidOps);
        }

        private static bool IsSuccess<T>(SectionJson<T> section) where T : AssignableRecordJson
        {
            HttpStatusCode status = (HttpStatusCode)section.Status;
            return status == HttpStatusCode.OK || status == HttpStatusCode.NoContent;
        }

        private static bool CanSynchronize<T>(SectionJson<T> section, List<InvalidOperationException> invalidOps)
            where T : AssignableRecordJson
        {
            if (IsSuccess(section))
                return true;
            else
            {
                invalidOps.Add(MakeException(section));
                return false;
            }
        }

        private static InvalidOperationException MakeException<T>(SectionJson<T> section)
            where T : AssignableRecordJson
        {
            return new(section.GetFirstMessage() + " -> " + section.GetFirstError());
        }

        public override string GetId()
        {
            return MakeId();
        }

        /// <summary>
        /// As of v1.0, it is currently a business decision to only allow users to interact with Cases and Incidents 
        /// from their caseload.
        /// </summary>
        /// <param name="caseloadItems"></param>
        /// <returns></returns>
        private IEnumerable<CaseloadItem> FilterNonCasesAndIncidents(IEnumerable<CaseloadItem> caseloadItems)
        {
            return caseloadItems.Where(item =>
            {
                EntityType type = item.EntityType.ParseEntityType();
                return type == EntityType.Case || type == EntityType.Incident;
            });
        }
    }
}
