using Oidc;
using Realms;
using Visitz.Extensions;
using Visitz.Services.Base;
using Visitz.Services.Messages;
using Visitz.Storage;
using VisitzModel.Extensions;
using VisitzModel.Models.Caseload;
using VisitzModel.Storage;

namespace Visitz.Services.Caseload;

internal class RecordCleanupService : VisitzService
{
    static readonly int MaxDaysThreshold = 7;

    DateTimeOffset dateThreshold;

    public static StartServiceMessage MakeStartMessage()
    {
        return new() { ServiceId = MakeId(), ServiceType = typeof(RecordCleanupService) };
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
    }

    async Task RemoveStaleOfficeRecordsAsync()
    {
        OidcSessionInfo info = await OidcSession.GetInfoAsync();
        if (string.IsNullOrWhiteSpace(info.Idir))
        {
            ResultCode = Result.NoOperation;
            return;
        }

        Realm realm = await VisitzRealms.GetIcmDataRealmAsync();
        UserIgnoredContentPrefs ignoredPrefs = ServiceProvider.GetService<UserIgnoredContentPrefs>();

        dateThreshold = DateTimeOffset.UtcNow.AddDays(-MaxDaysThreshold);

        IEnumerable<IBusinessObject> officeCases = CaseRecord
            .GetAllByAssignee(realm, info.Idir, isAssignedTo: false)
            .Where(IsStaleRecord);

        IEnumerable<IBusinessObject> officeIncidents = IncidentRecord
            .GetAllByAssignee(realm, info.Idir, isAssignedTo: false)
            .Where(IsStaleRecord);

        IEnumerable<IBusinessObject> staleOfficeRecords = officeCases.Concat(officeIncidents);

        await realm.CommitAsync(() =>
        {
            foreach (var stale in staleOfficeRecords)
                DeleteDependentData(realm, stale, ignoredPrefs);
        });

        ResultCode = Result.Successful;
    }

    bool IsStaleRecord(IBusinessObject bo)
    {
        return (bo.LocalState?.ShouldDownloadDuringRefresh ?? false) && bo.LocalState?.LastOpened < dateThreshold;
    }

    void DeleteDependentData(Realm realm, IBusinessObject businessObject, UserIgnoredContentPrefs ignoredPrefs)
    {
        try
        {
            businessObject.DeleteDependentData(ignoredPrefs, realm, deleteLocalState: false);
            businessObject.LocalState?.ShouldDownloadDuringRefresh = false;
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, "Deleting dependent data failed");
        }
    }
}
