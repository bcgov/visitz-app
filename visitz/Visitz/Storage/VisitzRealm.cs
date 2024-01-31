using Realms;
using Realms.Exceptions;
using Realms.Schema;
using Visitz.Models;
using Visitz.Models.SafetyAssess;
using Visitz.Storage.Migrations;

#if WINDOWS
using MauiFileSystem = Microsoft.Maui.Storage.FileSystem;
#endif

namespace Visitz.Storage;

public class VisitzRealm
{
    public static readonly ulong Version2_0 = 1;
    public static readonly ulong CurrentVersion = Version2_0;

    public static readonly string IcmDataCopiesPath = "icmDataCopies.realm";
    public static readonly string NoteDraftRealmPath = "noteDraftRealm.realm";
    public static readonly string SafetyAssessmentRealmPath = "safetyAssessmentRealmPath.realm";

    private static async Task<RealmConfiguration> MakeConfigAsync(string realmPath, RealmSchema schema)
    {
        try
        {
            return new RealmConfiguration(realmPath)
            {
                EncryptionKey = await VisitzKey.GetKey(realmPath),
                SchemaVersion = CurrentVersion,
                MigrationCallback = MigrateRealm,
                Schema = schema,
            };
        } 
        catch (RealmMigrationNeededException e)
        {
            ConsoleTrace.TraceMethod(typeof(VisitzRealm), $"Realm exception: {e.Message}, {e.StackTrace}");
            throw;
        }
    }

    private static void MigrateRealm(Migration migration, ulong oldSchemaVersion)
    {
        IcmDataMigrations.MigrateRealm(migration, oldSchemaVersion);
    }

    private static async Task<Realm> GetInstanceAsync(string realmFilename, RealmSchema schema)
    {
        var realmConfig = await MakeConfigAsync(GetRealmPath(realmFilename), schema);

        ConsoleTrace.TraceMethod(typeof(VisitzRealm), $"GetInstanceAsync('{realmConfig.DatabasePath}')");

        return await Realm.GetInstanceAsync(realmConfig);
    }

    private static async Task<Realm> ErrorNewInstanceAsync(string path, RealmSchema schema, Exception ex)
    {
#if DEBUG
        Console.WriteLine(ex.StackTrace);
#endif

        await Application.Current.MainPage.DisplayAlert(
            Resources.Localization.LocalizedStrings.RealmDatabaseErrorTitle,
            Resources.Localization.LocalizedStrings.RealmDatabaseErrorMessage,
            Resources.Localization.LocalizedStrings.Ok);

        DeleteRealm(path);
        return await GetInstanceAsync(path, schema);
    }

    private static void DeleteRealm(string path)
    {
        Realm.DeleteRealm(new RealmConfiguration(path));
    }

    public static void DeleteRealmKey(string path)
    {
        VisitzKey.RemoveKey(path);
    }

    private static async Task<Realm> GetAsync(string path, RealmSchema schema)
    {
        try
        {
            return await GetInstanceAsync(path, schema);
        }
        catch (RealmMismatchedConfigException ex)
        {
            return await ErrorNewInstanceAsync(path, schema, ex);
        }
        catch (RealmDecryptionFailedException ex)
        {
            return await ErrorNewInstanceAsync(path, schema, ex);
        }
        catch (RealmInvalidDatabaseException ex)
        {
            return await ErrorNewInstanceAsync(path, schema, ex);
        }
    }

    private static string GetRealmPath(string realmName)
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

    public static async Task ClearIcmDataRealm()
    {
        using var realm = await GetIcmDataAsync();
        await realm.WriteAsync(realm.RemoveAll);
    }

    public static async Task ClearSafetyAssessmentDraftRealm()
    {
        using var realm = await GetSafetyAssessmentDraftAsync();
        await realm.WriteAsync(realm.RemoveAll);
    }

    public static async Task<Realm> GetIcmDataAsync()
    {
        return await GetAsync(IcmDataCopiesPath, new[]
        {
            typeof(CaseloadItem),
            typeof(FamilyMember),
            typeof(NoteItem),
        });
    }

    public static async Task<Realm> GetNoteDraftAsync()
    {
        return await GetAsync(NoteDraftRealmPath, new[] { typeof(NoteDraft), });
    }

    public static async Task<Realm> GetSafetyAssessmentDraftAsync()
    {
        return await GetAsync(SafetyAssessmentRealmPath, schema: new[] 
        { 
            typeof(SafetyAssessment),
            typeof(FactorInfluence),
            typeof(ProtectiveCapacity),
            typeof(SafetyDecisions),
            typeof(SafetyFactors),
            typeof(SafetyInterventions),
        });
    }
}
