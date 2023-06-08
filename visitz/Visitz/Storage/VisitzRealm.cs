using Realms;
using Realms.Exceptions;

namespace Visitz.Storage
{
    public class VisitzRealm
    {
        private static readonly string DbName = "visitz.realm";

        private static async Task<Realm> GetInstanceAsync()
        {
            return await Realm.GetInstanceAsync(new RealmConfiguration(DbName)
            {
                EncryptionKey = await VisitzKey.GetKey(),
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

            Realm.DeleteRealm(new RealmConfiguration(DbName));
            return await GetInstanceAsync();
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
