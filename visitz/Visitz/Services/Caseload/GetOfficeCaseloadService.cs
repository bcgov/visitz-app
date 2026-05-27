using Oidc;
using Visitz.Resources.Localization;
using Visitz.Services.Base;
using Visitz.Services.Messages;
using Visitz.Storage;
using VisitzApi;
using VisitzApi.Requests;
using VisitzModel.Models.Caseload;
using VisitzModel.Storage;

namespace Visitz.Services.Caseload;

#nullable enable

internal class GetOfficeCaseloadService(Vpi vpi, LastUpdatedPrefs prefs, UserIgnoredContentPrefs ignoredPrefs)
    : ApiPaginationService(vpi, prefs)
{
    UserIgnoredContentPrefs UserIgnoredPrefs { get; } = ignoredPrefs;

    bool Force => (bool)Payload;

    List<CaseRecord> CaseRecords { get; } = [];

    List<IncidentRecord> IncidentRecords { get; } = [];

    // TODO: memos and SRs

    HashSet<string> Offices { get; } = [];

    public static string MakeId()
    {
        return nameof(GetOfficeCaseloadService);
    }

    public static StartServiceMessage MakeStartMessage(bool forceFullDownload)
    {
        return new()
        {
            ServiceId = MakeId(),
            ServiceType = typeof(GetOfficeCaseloadService),
            Payload = forceFullDownload,
        };
    }

    public override string GetId()
    {
        return MakeId();
    }

    protected override async Task<int> RunPaginatedService(Pagination pagination)
    {
        bool downloadAfter = !Force;

        if (downloadAfter && LastUpdatedPrefs.Get(GetId()) is DateTime after)
            pagination.After = (DateTimeOffset)after;

        await OidcSession.AssertValidSessionAsync(LocalizedStrings.NoInternet, CancelTokenSource.Token);
        var (total, officeCaseload) = await Vpi.GetOfficeCaseloadAsync(pagination: pagination);

        if (CaseloadHelper.CanSynchronize(officeCaseload.Cases, Exceptions))
            CaseRecords.AddRange(CaseRecord.FromApiJsonArray(officeCaseload.Cases.Items));

        if (CaseloadHelper.CanSynchronize(officeCaseload.Incidents, Exceptions))
            IncidentRecords.AddRange(IncidentRecord.FromApiJsonArray(officeCaseload.Incidents.Items));

        foreach (var office in officeCaseload.OfficeNames)
            Offices.Add(office);

        // TODO: memos and SRs

        return total;
    }

    protected override async Task AfterRun()
    {
        var sessionInfo = await OidcSession.GetInfoAsync();
        var username = sessionInfo.Idir;
        using var realm = await VisitzRealms.GetIcmDataRealmAsync();

        try
        {
            await IBusinessObject.SynchronizeAsync(
                realm,
                CaseRecords,
                UserIgnoredPrefs,
                username,
                isPersonalCaseload: false
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
                username,
                isPersonalCaseload: false
            );
        }
        catch (Exception ex)
        {
            Exceptions.Add(ex);
        }

        // TODO: memos and SRs

        try
        {
            Offices.UnionWith(CaseRecords.Select(@case => @case.ServiceOffice).Distinct());

            Offices.UnionWith(IncidentRecords.Select(incident => incident.ServiceOffice).Distinct());

            // TODO: memos and SRs

            sessionInfo.OfficeNames = Offices;
        }
        catch (Exception ex)
        {
            Exceptions.Add(ex);
        }
    }
}
