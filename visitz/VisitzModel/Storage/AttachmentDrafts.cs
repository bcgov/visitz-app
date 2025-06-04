using Realms;
using Realms.Schema;
using VisitzModel.Models.Attachments;

namespace VisitzModel.Storage;

public partial class AttachmentDrafts(byte[] encryptionKey) : VisitzRealmBase(Name, CurrentVersion, encryptionKey)
{
    public static readonly string Name = "attachmentDraftsRealm.realm";
    public static readonly ulong CurrentVersion = Version2_6_0;

    protected override RealmSchema MakeRealmSchema()
    {
        return new[]
        {
            typeof(Attachment),
            typeof(AttachmentDraft),
        };
    }

    protected override void MigrateRealm(Migration migration, ulong oldSchemaVersion)
    {
        // TODO...
    }
}
