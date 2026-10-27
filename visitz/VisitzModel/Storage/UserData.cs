using Realms;
using Realms.Schema;
using VisitzModel.Models.People;

namespace VisitzModel.Storage;

/// <summary>
/// <para>A realm meant for storing all user-generated data (including both data meant for local only or to be sent upstream).</para>
/// <para>Note: previously, user drafts would be stored in dedicated realms. Going forward, all drafts should be
/// stored in this realm instead. Also (when applicable) data should be migrated from those other realms to this
/// one.</para>
/// </summary>
/// <param name="encryptionKey"></param>
public class UserData(byte[] encryptionKey) : VisitzRealmBase(Name, CurrentVersion, encryptionKey)
{
    public static readonly string Name = "userData.realm";

    public static readonly ulong CurrentVersion = Version3_1_0;

    protected override RealmSchema MakeRealmSchema()
    {
        return new[]
        {
            typeof(ContactEducation),
            typeof(ContactEducationDraft),
            typeof(ContactLanguage),
            typeof(ContactLanguageDraft),
            typeof(ContactMedicalBehavioral),
            typeof(ContactMedicalBehavioralDraft),
        };
    }

    protected override void MigrateRealm(Migration migration, ulong oldSchemaVersion)
    {
        // Implement when migrations needed.
    }
}
