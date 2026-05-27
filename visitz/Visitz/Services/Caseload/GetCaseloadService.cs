using Oidc;
using Visitz.Services.Base;
using Visitz.Services.Messages;
using Visitz.Storage;
using VisitzApi;
using VisitzApi.Requests;
using VisitzModel.Models.Caseload;
using VisitzModel.Storage;

#nullable enable

namespace Visitz.Services.Caseload
{
    internal class GetCaseloadService(Vpi vpi, LastUpdatedPrefs prefs, UserIgnoredContentPrefs userIgnoredContentPrefs)
        : ApiPaginationService(vpi, prefs)
    {
        UserIgnoredContentPrefs UserIgnoredPrefs { get; } = userIgnoredContentPrefs;

        List<CaseRecord> CaseRecords { get; } = [];

        List<IncidentRecord> IncidentRecords { get; } = [];

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

        public override string GetId()
        {
            return MakeId();
        }

        protected override async Task<int> RunPaginatedService(Pagination pagination)
        {
            pagination.After = Force ? null : (DateTimeOffset?)LastUpdatedPrefs.Get(GetId());

            var (total, caseloadFromApi) = await Vpi.GetCaseloadAsync(pagination: pagination);

            var session = await OidcSessionInfo.GetAsync();
            using var realm = await VisitzRealms.GetIcmDataRealmAsync();

            List<Exception> invalidOps = [];

            if (CaseloadHelper.CanSynchronize(caseloadFromApi.Cases, invalidOps))
                CaseRecords.AddRange(CaseRecord.FromApiJsonArray(caseloadFromApi.Cases.Items, session.Idir));

            if (CaseloadHelper.CanSynchronize(caseloadFromApi.Incidents, invalidOps))
                IncidentRecords.AddRange(
                    IncidentRecord.FromApiJsonArray(caseloadFromApi.Incidents.Items, session.Idir)
                );

            // TODO: synchronize memos and service requests once we have official UI support

            if (invalidOps.Count > 0)
                throw new AggregateException(invalidOps);

            return total;
        }

        protected override async Task AfterRun()
        {
            var session = await OidcSessionInfo.GetAsync();
            using var realm = await VisitzRealms.GetIcmDataRealmAsync();

            try
            {
                await IBusinessObject.SynchronizeAsync(
                    realm,
                    CaseRecords,
                    UserIgnoredPrefs,
                    session.Idir,
                    isPersonalCaseload: true
                );
            }
            catch (Exception ex)
            {
                Exceptions.Add(ex);
            }

            try
            {
                await IBusinessObject.SynchronizeAsync(
                    realm,
                    IncidentRecords,
                    UserIgnoredPrefs,
                    session.Idir,
                    isPersonalCaseload: true
                );
            }
            catch (Exception ex)
            {
                Exceptions.Add(ex);
            }

            // TODO: synchronize memos and service requests once we have official UI supports
        }
    }
}
