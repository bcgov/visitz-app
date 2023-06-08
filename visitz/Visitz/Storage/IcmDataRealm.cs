using Realms;
using Realms.Exceptions;

namespace Visitz.Storage
{
    public class IcmDataRealm
    {
        private static readonly string Path = "icmDataCopies.realm";

        private static async Task<Realm> GetInstanceAsync()
        {
            return await Realm.GetInstanceAsync(new RealmConfiguration(Path)
            {
                EncryptionKey = await VisitzKey.GetKey(Path),
#if DEBUG
                ShouldDeleteIfMigrationNeeded = true
#endif
            });
        }

        private static async Task<Realm> ErrorNewInstanceAsync(Exception ex)
        {
#if DEBUG
            Console.WriteLine(ex.StackTrace);
#endif

            await Application.Current.MainPage.DisplayAlert(
                Resources.Localization.LocalizedStrings.RealmDatabaseErrorTitle,
                Resources.Localization.LocalizedStrings.RealmDatabaseErrorMessage,
                Resources.Localization.LocalizedStrings.Ok);

            DeleteRealm();
            return await GetInstanceAsync();
        }

        private static void DeleteRealm()
        {
            Realm.DeleteRealm(new RealmConfiguration(Path));
        }

        public static void DeleteRealmKey()
        {
            VisitzKey.RemoveKey(Path);
        }

        public static async Task<Realm> GetAsync()
        {
            try
            {
                return await GetInstanceAsync();
            }
            catch (RealmMismatchedConfigException ex)
            {
                return await ErrorNewInstanceAsync(ex);
            }
            catch (RealmDecryptionFailedException ex)
            {
                return await ErrorNewInstanceAsync(ex);
            }
            catch (RealmInvalidDatabaseException ex)
            {
                return await ErrorNewInstanceAsync(ex);
            }
        }
    }
}
