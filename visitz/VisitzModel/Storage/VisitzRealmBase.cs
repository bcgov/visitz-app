using Realms;
using Realms.Schema;

#if WINDOWS
using MauiFileSystem = Microsoft.Maui.Storage.FileSystem;
#endif

namespace VisitzModel.Storage;

public abstract class VisitzRealmBase
{
    public static readonly ulong Version2_0 = 1;
	public static readonly ulong Version2_3_3 = 2;

	public string RealmName { get; private set; }

    public ulong Version { get; private set; }

    public byte[] EncryptionKey { get; private set; }

    protected VisitzRealmBase(string realmName, ulong version, byte[] encryptionKey)
    {
        RealmName = realmName;
        Version = version;
        EncryptionKey = encryptionKey;
    }

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

    public async Task<Realm> GetAsync()
    {
        RealmConfiguration realmConfig;

        try
        {
            realmConfig = MakeRealmConfiguration();

            ConsoleTrace.TraceMethod(typeof(VisitzRealmBase), $"GetAsync('{realmConfig.DatabasePath}')");

            return await Realm.GetInstanceAsync(realmConfig);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"{ex.GetType()}: Unable to open Realm '{RealmName}/{Version}'", ex);
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
