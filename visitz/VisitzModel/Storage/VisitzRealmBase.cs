using Microsoft.Extensions.Logging;
using Realms;
using Realms.Schema;
using VisitzModel.Extensions;
#if WINDOWS
using VisitzModel.Platforms.Windows.Logging;
using MauiFileSystem = Microsoft.Maui.Storage.FileSystem;
#endif

namespace VisitzModel.Storage;

public abstract class VisitzRealmBase(string realmName, ulong version, byte[] encryptionKey)
{
    public static readonly ulong Version2_0 = 1;
    public static readonly ulong Version2_3_3 = 2;
    public static readonly ulong Version2_6_0 = 3;
    public static readonly ulong Version2_7_0 = 4;
    public static readonly ulong Version2_7_1 = 5;
    public static readonly ulong Version2_8_0 = 6;
    public static readonly ulong Version3_0_0 = 7;
    public static readonly ulong Version3_1_0 = 8;

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

    protected abstract RealmSchema MakeRealmSchema();

    protected abstract void MigrateRealm(Migration migration, ulong oldSchemaVersion);

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

    public async Task<Realm> GetAsync(ILogger? logger = null)
    {
        RealmConfiguration? realmConfig = null;

        try
        {
            realmConfig = MakeRealmConfiguration();

#if DEBUG
            if (ShouldUseLoggerInGetAsync)
                logger?.TraceMethod(this, Path.GetFileName(realmConfig.DatabasePath));
#endif

            return await Realm.GetInstanceAsync(realmConfig);
        }
        catch (Exception ex)
        {
            string message =
                $"{ex.GetType()}: Unable to open Realm '{RealmName}/{Version}' from path '{realmConfig?.DatabasePath ?? "<path not available>"}'";

            var invalidOpExeption = new InvalidOperationException(message, ex);

            if (ShouldUseLoggerInGetAsync)
                logger?.LogException(invalidOpExeption, message);
#if WINDOWS
            else
            {
                string category = GetType()?.FullName ?? typeof(VisitzRealmBase).FullName ?? "<type not available>";
                EventLogWriter.WriteEntry(LogLevel.Error, message, category, exception: invalidOpExeption);
            }
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

    internal static void MapAll<TNewType>(
        string oldTypeName,
        Migration migration,
        Action<TNewType, IRealmObject> mapper
    )
        where TNewType : IRealmObject
    {
        var olds = migration.OldRealm.DynamicApi.All(oldTypeName).ToList();
        var news = migration.NewRealm.All<TNewType>().ToList();

        for (int i = 0; i < news.Count; i++)
            mapper(news[i], olds[i]);
    }
}
