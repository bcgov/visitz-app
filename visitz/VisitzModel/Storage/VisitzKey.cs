using System.Security.Cryptography;

namespace VisitzModel.Storage
{
    public static class VisitzKey
    {
        private static readonly int DefaultKeySize = 64;

        private static readonly string EncryptionKeyName = "visitz.encryption.key.";

        private static byte[] NewKey(int keySize)
        {
            var encryptionKey = new byte[keySize];
            using var rng = RandomNumberGenerator.Create();

            rng.GetBytes(encryptionKey);
            return encryptionKey;
        }

        private static async Task SetKeyInStorage(string keyName, byte[] encryptionKey)
        {
            var encodedKey = Convert.ToBase64String(encryptionKey);

            await SecureStorage.Default.SetAsync(EncryptionKeyName + keyName, encodedKey);
        }

        private static async Task<byte[]> GetKeyFromStorage(string keyName)
        {
            var encodedKey = await SecureStorage.Default.GetAsync(EncryptionKeyName + keyName);

            return encodedKey != null
                ? Convert.FromBase64String(encodedKey)
                : null;
        }

        public static async Task<byte[]> GetKey(string keyName, int? keySizeIfNew = null)
        {
            byte[] encryptionKey = await GetKeyFromStorage(keyName);

            if (encryptionKey == null)
            {
                await SetKeyInStorage(keyName, NewKey(keySizeIfNew ?? DefaultKeySize));
                encryptionKey = await GetKeyFromStorage(keyName);
            }

            return encryptionKey;
        }

        public static void RemoveKey(string keyName)
        {
            SecureStorage.Default.Remove(EncryptionKeyName + keyName);
        }
    }
}
