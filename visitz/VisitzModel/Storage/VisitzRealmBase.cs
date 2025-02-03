using Microsoft.Extensions.Logging;
using Realms;
using Realms.Schema;

#if WINDOWS
using VisitzModel.Platforms.Windows.Logging;
using MauiFileSystem = Microsoft.Maui.Storage.FileSystem;
#endif

namespace VisitzModel.Storage;

public abstract class VisitzRealmBase(string realmName, ulong version, byte[] encryptionKey)
{
    public static readonly ulong Version2_0 = 1;
    public static readonly ulong Version2_3_3 = 2;

    public string RealmName { get; private set; } = realmName;

    public ulong Version { get; private set; } = version;

    public byte[] EncryptionKey { get; private set; } = encryptionKey;

    protected bool ShouldUseLoggerInGetAsync = true;

    public static string GetRealmPath(string realmName)
    {
#if WINDOWS
        // Explicitly declare a path, otherwise it's put into system32.
        return Path.Combine(MauiFileSystem.Current.AppDataDirectory, realmName);
#else
        // For non-Windows envs, we'll continue to rely on Realm's default path it constructs when
        // no path is provided in the RealmConfiguration (for backwards compatibility).
        return realmName;
#endif
    }

    abstract protected RealmSchema MakeRealmSchema();

    abstract protected void MigrateRealm(Migration migration, ulong oldSchemaVersion);

    private RealmConfiguration MakeRealmConfiguration()
    {
        return new(GetRealmPath(RealmName))
        {
            EncryptionKey = EncryptionKey,
            SchemaVersion = Version,
            MigrationCallback = MigrateRealm,
            Schema = MakeRealmSchema(),
        };
    }

    public async Task<Realm> GetAsync(ILogger logger = null)
    {
        RealmConfiguration realmConfig = null;

        try
        {
            realmConfig = MakeRealmConfiguration();
            
            ConsoleTrace.TraceMethod(typeof(VisitzRealmBase), Path.GetFileName(realmConfig.DatabasePath));

            return await Realm.GetInstanceAsync(realmConfig);
        }
        catch (Exception ex)
        {
            string message = $"{ex.GetType()}: Unable to open Realm '{RealmName}/{Version}' from path '{realmConfig?.DatabasePath ?? "<path not available>"}'";

            var invalidOpExeption = new InvalidOperationException(message, ex);

            if (ShouldUseLoggerInGetAsync)
                logger?.LogError(invalidOpExeption, message);
#if WINDOWS
            else
                EventLogWriter.WriteEntry(LogLevel.Error, message, GetType().FullName, exception: invalidOpExeption);
#endif

            throw invalidOpExeption;
        }
    }

    public async Task ClearAllData()
    {
        using var realm = await GetAsync();
        await realm.WriteAsync(realm.RemoveAll);
    }

    public void DeleteRealm()
    {
        Realm.DeleteRealm(new RealmConfiguration(GetRealmPath(RealmName)));
    }
}
