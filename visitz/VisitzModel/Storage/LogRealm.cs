using Realms;
using Realms.Schema;
using VisitzModel.Models;

namespace VisitzModel.Storage;

public class LogRealm(byte[] encryptionKey) : VisitzRealmBase(Name, CurrentVersion, encryptionKey)
{
    public static readonly string Name = "LogRealm.realm";
    public static readonly ulong CurrentVersion = Version2_3_3;

    protected override RealmSchema MakeRealmSchema()
    {
        return new[] { typeof(LogEntry), };
    }

    protected override void MigrateRealm(Migration migration, ulong oldSchemaVersion)
    {
        
    }
}
