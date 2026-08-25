using Realms;
using Realms.Schema;
using VisitzModel.Models.Logging;

namespace VisitzModel.Storage;

public class LogRealm : VisitzRealmBase
{
    public static readonly string Name = "LogRealm.realm";
    public static readonly ulong CurrentVersion = Version3_1_0;

    public LogRealm(byte[] encryptionKey)
        : base(Name, CurrentVersion, encryptionKey)
    {
        ShouldUseLoggerInGetAsync = false;
    }

    protected override RealmSchema MakeRealmSchema()
    {
        return new[] { typeof(LogEntry) };
    }

    protected override void MigrateRealm(Migration migration, ulong oldSchemaVersion)
    {
        if (oldSchemaVersion < Version3_0_0)
        {
            MapAll<LogEntry>(
                "LogEntry",
                migration,
                (n, o) =>
                {
                    n.LevelText = o.DynamicApi.Get<string>("Type");
                    n.Message = o.DynamicApi.Get<string>("Message");
                    n.Source = o.DynamicApi.Get<string>("Source");
                    n.Timestamp = o.DynamicApi.Get<DateTimeOffset>("Timestamp");
                }
            );
        }
    }
}
