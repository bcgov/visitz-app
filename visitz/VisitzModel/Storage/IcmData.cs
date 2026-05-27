using Realms;
using Realms.Schema;
using VisitzModel.Models.Attachments;
using VisitzModel.Models.CallDetails;
using VisitzModel.Models.Caseload;
using VisitzModel.Models.InPersonVisits;
using VisitzModel.Models.Notes;
using VisitzModel.Models.People;
using VisitzModel.Models.SafetyAssess;
using VisitzModel.Storage.Migrations;

namespace VisitzModel.Storage;

public class IcmData(byte[] encryptionKey) : VisitzRealmBase(Name, CurrentVersion, encryptionKey)
{
    public static readonly string Name = "icmDataCopies.realm";
    public static readonly ulong CurrentVersion = Version2_8_0;

    protected override RealmSchema MakeRealmSchema()
    {
        return new[]
        {
            typeof(CaseRecord),
            typeof(IncidentRecord),
            typeof(MemoRecord),
            typeof(ServiceRequestRecord),
            typeof(NoteItem),
            typeof(PersonVisit),
            typeof(IcmContact),
            typeof(SupportNetworkItem),
            typeof(Attachment),
            typeof(AttachmentDraft),
            typeof(BoLocalState),
            typeof(SafetyAssessment),
            typeof(FactorInfluence),
            typeof(ProtectiveCapacity),
            typeof(SafetyDecisions),
            typeof(SafetyFactors),
            typeof(SafetyInterventions),
            typeof(IncidentConcerns),
            typeof(AdditionalInformation),
            typeof(CallInformation),
            typeof(ContactMedicalBehavioral),
            typeof(ContactLegalAuthority),
            typeof(ContactLanguage),
            typeof(ContactEducation),
        };
    }

    protected override void MigrateRealm(Migration migration, ulong oldSchemaVersion)
    {
        IcmDataMigrations.MigrateRealm(migration, oldSchemaVersion);
    }

    public static async Task<string> GetStats(Realm realm, string username)
    {
        string stats = "";

        var cases = realm.All<CaseRecord>().ToList();
        var incidents = realm.All<IncidentRecord>().ToList();
        // TODO: Memos and SRs

        AppendInfoLine(ref stats, "All records available", cases.Count + incidents.Count);
        stats += Environment.NewLine;

        // All assigned
        var assignedCases = CaseRecord.GetAllByAssignee(realm, username, isAssignedTo: true);
        var assignedIncidents = IncidentRecord.GetAllByAssignee(realm, username, isAssignedTo: true);
        int assignedCaseCount = assignedCases.Count();
        int assignedIncidentCount = assignedIncidents.Count();

        AppendInfoLine(ref stats, "Assigned records", assignedCaseCount + assignedIncidentCount);
        AppendInfoLine(ref stats, "Assigned cases", assignedCaseCount);
        AppendInfoLine(ref stats, "Assigned incidents", assignedIncidentCount);
        stats += Environment.NewLine;

        // All office
        var officeCases = CaseRecord.GetAllByAssignee(realm, username, isAssignedTo: false).ToList();
        var officeIncidents = IncidentRecord.GetAllByAssignee(realm, username, isAssignedTo: false).ToList();

        AppendInfoLine(ref stats, "Available office records", officeCases.Count + officeIncidents.Count);
        AppendInfoLine(ref stats, "Available office cases", officeCases.Count);
        AppendInfoLine(ref stats, "Available office incidents", officeIncidents.Count);
        stats += Environment.NewLine;

        // All downloaded to device
        var downloadedOfficeCases = officeCases.Where(c =>
            c.LocalState != null && c.LocalState.ShouldDownloadDuringRefresh
        );
        var downloadedOfficeIncidents = officeIncidents.Where(i =>
            i.LocalState != null && i.LocalState.ShouldDownloadDuringRefresh
        );
        int downloadedOfficeCasesCount = downloadedOfficeCases.Count();
        int downloadedOfficeIncidentsCount = downloadedOfficeIncidents.Count();

        AppendInfoLine(
            ref stats,
            "Downloaded office records",
            downloadedOfficeCasesCount + downloadedOfficeIncidentsCount
        );
        AppendInfoLine(ref stats, "Downloaded office cases", downloadedOfficeCasesCount);
        AppendInfoLine(ref stats, "Downloaded office incidents", downloadedOfficeIncidentsCount);
        stats += Environment.NewLine;

        return stats;
    }

    static void AppendInfoLine(ref string text, string description, object value)
    {
        text += description + Environment.NewLine + "=> " + value.ToString() + Environment.NewLine;
    }
}
