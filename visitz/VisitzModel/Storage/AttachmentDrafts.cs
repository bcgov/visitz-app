using Realms;
using Realms.Schema;
using VisitzModel.Models.Attachments;
using VisitzModel.Storage.Migrations;

namespace VisitzModel.Storage;

public partial class AttachmentDrafts(byte[] encryptionKey) : VisitzRealmBase(Name, CurrentVersion, encryptionKey)
{
    public static readonly string Name = "attachmentDraftsRealm.realm";
    public static readonly ulong CurrentVersion = Version3_0_0;

    protected override RealmSchema MakeRealmSchema()
    {
        return new[] { typeof(Attachment), typeof(AttachmentDraft) };
    }

    protected override void MigrateRealm(Migration migration, ulong oldSchemaVersion)
    {
        AttachmentMigrations.MigrateRealm(migration, oldSchemaVersion);

        if (oldSchemaVersion < Version3_0_0)
        {
            MapAll<AttachmentDraft>(
                "AttachmentDraft",
                migration,
                (n, o) =>
                {
                    n.RelatedEntityId = o.DynamicApi.Get<string>("RelatedEntityId") ?? string.Empty;
                    n.RelatedEntityTypeInt = o.DynamicApi.Get<int>("RelatedEntityTypeInt");
                    n.RelatedEntitySubtypeInt = o.DynamicApi.Get<int>("RelatedEntitySubtypeInt");
                    n.DraftCreated = o.DynamicApi.Get<DateTimeOffset>("DraftCreated");
                    n.LastUpdated = o.DynamicApi.Get<DateTimeOffset>("LastUpdated");
                    n.DraftLocation = o.DynamicApi.Get<string>("DraftLocation") ?? string.Empty;
                    // No need to migrate n.Attachment itself, it's already linked

                    n.Attachment?.CreatedDate = n.DraftCreated;
                    n.Attachment?.UpdatedDate = n.LastUpdated;
                }
            );
        }
    }
}
