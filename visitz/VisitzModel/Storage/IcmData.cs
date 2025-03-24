using Realms;
using Realms.Schema;
using VisitzModel.Models;
using VisitzModel.Models.Attachments;
using VisitzModel.Models.Caseload;
using VisitzModel.Models.InPersonVisits;
using VisitzModel.Models.Notes;
using VisitzModel.Models.People;
using VisitzModel.Storage.Migrations;

namespace VisitzModel.Storage;

public class IcmData(byte[] encryptionKey) : VisitzRealmBase(Name, CurrentVersion, encryptionKey)
{
    public static readonly string Name = "icmDataCopies.realm";
    public static readonly ulong CurrentVersion = Version2_6_0;

    protected override RealmSchema MakeRealmSchema()
    {
        return new[]
        {
            typeof(CaseloadItem),
            typeof(FamilyMember),
            typeof(NoteItem),
            typeof(PersonVisit),
            typeof(CaseRecord),
            typeof(IncidentRecord),
            typeof(MemoRecord),
            typeof(IcmContact),
            typeof(SupportNetworkItem),
            typeof(Attachment),
            typeof(AttachmentDraft),
        };
    }

    protected override void MigrateRealm(Migration migration, ulong oldSchemaVersion)
    {
        IcmDataMigrations.MigrateRealm(migration, oldSchemaVersion);
    }
}
