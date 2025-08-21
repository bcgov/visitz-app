using Microsoft.Extensions.Logging;
using Oidc;
using Realms;
using Visitz.Services.Base;
using Visitz.Services.Messages;
using Visitz.Storage;
using VisitzModel.Extensions;
using VisitzModel.Models.Caseload;
using VisitzModel.Storage;

namespace Visitz.Services.Caseload;

#nullable enable

internal class RecordCleanupService : VisitzService
{
    static readonly int MaxDaysThreshold = 7;

    public static StartServiceMessage MakeStartMessage()
    {
        return new()
        {
            ServiceId = MakeId(),
            ServiceType = typeof(RecordCleanupService),
        };
    }

    public static string MakeId()
    {
        return nameof(RecordCleanupService);
    }

    public override string GetId()
    {
        return MakeId();
    }

    protected override async Task RunServiceAsync()
    {
        await RemoveStaleOfficeRecordsAsync();

        ResultCode = Result.Successful;
    }

    async Task RemoveStaleOfficeRecordsAsync()
    {
        OidcSessionInfo info = await OidcSession.GetInfoAsync().ConfigureAwait(false);
        Realm realm = await VisitzRealms.GetIcmDataRealmAsync().ConfigureAwait(false);
        UserIgnoredContentPrefs ignoredPrefs = ServiceProvider.GetService<UserIgnoredContentPrefs>();

        DateTimeOffset dateThreshold = DateTimeOffset.UtcNow.AddDays(-MaxDaysThreshold);

        bool predicate(IBusinessObject bo) =>
            (bo.LocalState?.ShouldDownloadDuringRefresh ?? false)
                && bo.LocalState.LastOpened < dateThreshold;

        IEnumerable<IBusinessObject> officeCases = CaseRecord
            .GetAllByAssignee(realm, info.Idir, invert: true)
            .Where(predicate);

        IEnumerable<IBusinessObject> officeIncidents = IncidentRecord
            .GetAllByAssignee(realm, info.Idir, invert: true)
            .Where(predicate);

        IEnumerable<IBusinessObject> staleOfficeRecords = officeCases.Concat(officeIncidents);

        await realm.CommitAsync(() =>
        {
            foreach (var stale in staleOfficeRecords)
                DeleteDependentData(realm, stale, ignoredPrefs);
        });
    }

    void DeleteDependentData(
        Realm realm,
        IBusinessObject businessObject,
        UserIgnoredContentPrefs ignoredPrefs)
    {
        try
        {
            businessObject.DeleteDependentData(ignoredPrefs, realm, deleteLocalState: false);
            businessObject.LocalState.ShouldDownloadDuringRefresh = false;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Deleting dependent data failed");
        }
    }
}
