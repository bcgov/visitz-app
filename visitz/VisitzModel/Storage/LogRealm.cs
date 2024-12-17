using Realms;
using Realms.Schema;
using VisitzModel.Models;

namespace VisitzModel.Storage;

public class LogRealm : VisitzRealmBase
{
    public static readonly string Name = "LogRealm.realm";
    public static readonly ulong CurrentVersion = Version2_3_3;

    public LogRealm(byte[] encryptionKey) : base(Name, CurrentVersion, encryptionKey)
    {
        ShouldUseLoggerInGetAsync = false;
    }

    protected override RealmSchema MakeRealmSchema()
    {
        return new[] { typeof(LogEntry), };
    }

    protected override void MigrateRealm(Migration migration, ulong oldSchemaVersion)
    {
        
    }
}
