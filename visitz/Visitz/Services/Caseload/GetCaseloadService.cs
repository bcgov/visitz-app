using System.Net;
using Visitz.Services.Base;
using Visitz.Services.Messages;
using Visitz.Storage;
using VisitzApi;
using VisitzApi.Models.Base;
using VisitzApi.Models.Caseload;
using VisitzModel.Models.Caseload;
using VisitzModel.Storage;

namespace Visitz.Services.Caseload
{
    public class GetCaseloadService(
        Vpi vpi,
        LastUpdatedPrefs prefs,
        UserIgnoredContentPrefs userIgnoredContentPrefs) : VisitzApiService(vpi, prefs)
    {
        UserIgnoredContentPrefs UserIgnoredPrefs { get; } = userIgnoredContentPrefs;
        public static string MakeId()
        {
            return nameof(GetCaseloadService);
        }

        public static StartServiceMessage MakeStartMessage(bool forceDownload)
        {
            return new StartServiceMessage
            {
                ServiceId = MakeId(),
                ServiceType = typeof(GetCaseloadService),
                Payload = forceDownload,
            };
        }

        public bool Force => (bool)Payload;

        protected override async Task RunApiServiceAsync()
        {
            await DownloadAndSaveCaseloadV2Async();

            ResultCode = Result.Successful;
        }

        private async Task DownloadAndSaveCaseloadV2Async()
        {
            DateTimeOffset? after = Force ? null : (DateTimeOffset?)LastUpdatedPrefs.Get(GetId());

            CaseloadJson caseloadFromApi = await Vpi.GetCaseloadAsync(after: after);

            using var realm = await VisitzRealms.GetIcmDataRealmAsync();

            List<InvalidOperationException> invalidOps = [];

            if (CanSynchronize(caseloadFromApi.Cases, invalidOps))
                await CaseRecord.SynchronizeCasesAsync(realm, caseloadFromApi.Cases, UserIgnoredPrefs);

            if (CanSynchronize(caseloadFromApi.Incidents, invalidOps))
                await IncidentRecord.SynchronizeAsync(realm, caseloadFromApi.Incidents, UserIgnoredPrefs);

            // TODO: synchronize memos and service requests once we have official UI support

            if (invalidOps.Count > 0)
                throw new AggregateException(invalidOps);
        }

        private static bool IsSuccess<T>(SectionJson<T> section) where T : AssignableRecordJson
        {
            HttpStatusCode status = (HttpStatusCode)section.Status;
            return status == HttpStatusCode.OK || status == HttpStatusCode.NoContent;
        }

        private static bool CanSynchronize<T>(
            SectionJson<T> section,
            List<InvalidOperationException> invalidOps)
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
    }
}
