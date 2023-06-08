using System.Security.Cryptography;

namespace Visitz.Storage
{
    public static class VisitzKey
    {
        private static readonly int KeySize = 64;

        private static readonly string EncryptionKeyName = "visitz.encryption.key";

        private static byte[] NewKey()
        {
            var encryptionKey = new byte[KeySize];
            using var rng = RandomNumberGenerator.Create();

            rng.GetBytes(encryptionKey);
            return encryptionKey;
        }

        private static async Task SetKeyInStorage(byte[] encryptionKey)
        {
            var encodedKey = Convert.ToBase64String(encryptionKey);

            await SecureStorage.Default.SetAsync(EncryptionKeyName, encodedKey);
        }

        private static async Task<byte[]> GetKeyFromStorage()
        {
            var encodedKey = await SecureStorage.Default.GetAsync(EncryptionKeyName);

            return encodedKey != null 
                ? Convert.FromBase64String(encodedKey)
                : null;
        }

        public static async Task<byte[]> GetKey()
        {
            byte[] encryptionKey = await GetKeyFromStorage();

            if (encryptionKey == null)
            {
                await SetKeyInStorage(NewKey());
                encryptionKey = await GetKeyFromStorage();
            }

            return encryptionKey;
        }

        public static void RemoveKey()
        {
            SecureStorage.Default.Remove(EncryptionKeyName);
        }
    }
}
