using Realms;
using Realms.Exceptions;

namespace Visitz.Storage;

public class VisitzRealm
{
    public static readonly string IcmDataCopiesPath = "icmDataCopies.realm";
    public static readonly string NoteDraftRealmPath = "noteDraftRealm.realm";

    private static async Task<Realm> GetInstanceAsync(string path)
    {
        return await Realm.GetInstanceAsync(new RealmConfiguration(path)
        {
            EncryptionKey = await VisitzKey.GetKey(path),
#if DEBUG
            ShouldDeleteIfMigrationNeeded = true
#endif
        });
    }

    private static async Task<Realm> ErrorNewInstanceAsync(string path, Exception ex)
    {
#if DEBUG
        Console.WriteLine(ex.StackTrace);
#endif

        await Application.Current.MainPage.DisplayAlert(
            Resources.Localization.LocalizedStrings.RealmDatabaseErrorTitle,
            Resources.Localization.LocalizedStrings.RealmDatabaseErrorMessage,
            Resources.Localization.LocalizedStrings.Ok);

        DeleteRealm(path);
        return await GetInstanceAsync(path);
    }

    private static void DeleteRealm(string path)
    {
        Realm.DeleteRealm(new RealmConfiguration(path));
    }

    public static void DeleteRealmKey(string path)
    {
        VisitzKey.RemoveKey(path);
    }

    private static async Task<Realm> GetAsync(string path)
    {
        try
        {
            return await GetInstanceAsync(path);
        }
        catch (RealmMismatchedConfigException ex)
        {
            return await ErrorNewInstanceAsync(path, ex);
        }
        catch (RealmDecryptionFailedException ex)
        {
            return await ErrorNewInstanceAsync(path, ex);
        }
        catch (RealmInvalidDatabaseException ex)
        {
            return await ErrorNewInstanceAsync(path, ex);
        }
    }

    public static async Task<Realm> GetIcmDataAsync()
    {
        return await GetAsync(IcmDataCopiesPath);
    }

    public static async Task<Realm> GetNoteDraftAsync()
    {
        return await GetAsync(NoteDraftRealmPath);
    }
}
