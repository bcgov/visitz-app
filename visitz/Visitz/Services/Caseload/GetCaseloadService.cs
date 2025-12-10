using Microsoft.Extensions.Logging;
using Oidc;
using Visitz.Services.Base;
using Visitz.Services.Messages;
using Visitz.Storage;
using VisitzApi;
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
            try
            {
                await DownloadAndSaveCaseloadV2Async();

                ResultCode = Result.Successful;
                LastUpdatedPrefs.SetUtcNow(AutoRefreshService.CooldownTimestampUtc);
            }
            catch (OperationCanceledException opEx)
            {
                Logger.LogInformation($"Caseload refresh cancelled: '{opEx.Message}'");
            }
            catch (Exception)
            {
                LastUpdatedPrefs.SetUtcNow(AutoRefreshService.CooldownTimestampUtc);
                throw;
            }
        }

        private async Task DownloadAndSaveCaseloadV2Async()
        {
            DateTimeOffset? after = Force ? null : (DateTimeOffset?)LastUpdatedPrefs.Get(GetId());

            CaseloadJson caseloadFromApi = await Vpi.GetCaseloadAsync(after: after);

            var session = await OidcSessionInfo.GetAsync();
            using var realm = await VisitzRealms.GetIcmDataRealmAsync();

            List<Exception> invalidOps = [];

            if (CaseloadHelper.CanSynchronize(caseloadFromApi.Cases, invalidOps))
                await CaseRecord.SynchronizeAsync(
                    realm,
                    caseloadFromApi.Cases.Items,
                    UserIgnoredPrefs,
                    session.Idir,
                    isPersonalCaseload: true);

            if (CaseloadHelper.CanSynchronize(caseloadFromApi.Incidents, invalidOps))
                await IncidentRecord.SynchronizeAsync(
                    realm,
                    caseloadFromApi.Incidents.Items,
                    UserIgnoredPrefs,
                    session.Idir,
                    isPersonalCaseload: true);

            // TODO: synchronize memos and service requests once we have official UI support

            if (invalidOps.Count > 0)
                throw new AggregateException(invalidOps);
        }

        public override string GetId()
        {
            return MakeId();
        }
    }
}
