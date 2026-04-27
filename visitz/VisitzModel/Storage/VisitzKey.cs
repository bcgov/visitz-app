using System.Security.Cryptography;

namespace VisitzModel.Storage;

public static class VisitzKey
{
    private static readonly int _defaultKeySize = 64;

    private static readonly string _encryptionKeyName = "visitz.encryption.key.";

    private static readonly SemaphoreSlim _semaphore = new(1);

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
        var namespacedKey = _encryptionKeyName + keyName;

        await SecureStorage.Default.SetAsync(namespacedKey, encodedKey);
    }

    private static async Task<byte[]?> GetKeyFromStorage(string keyName)
    {
        var namespacedKey = _encryptionKeyName + keyName;
        var encodedKey = await SecureStorage.Default.GetAsync(namespacedKey);

        return encodedKey != null ? Convert.FromBase64String(encodedKey) : null;
    }

    public static async Task<byte[]?> GetKey(string keyName, int? keySizeIfNew = null)
    {
        await _semaphore.WaitAsync();

        try
        {
            return await DoGetKey(keyName, keySizeIfNew);
        }
        finally
        {
            try
            {
                _semaphore.Release();
            }
            catch { }
        }
    }

    static async Task<byte[]?> DoGetKey(string keyName, int? keySizeIfNew = null)
    {
        byte[]? encryptionKey = await GetKeyFromStorage(keyName);

        if (encryptionKey == null)
        {
            await SetKeyInStorage(keyName, NewKey(keySizeIfNew ?? _defaultKeySize));
            encryptionKey = await GetKeyFromStorage(keyName);
        }

        return encryptionKey;
    }

    public static void RemoveKey(string keyName)
    {
        SecureStorage.Default.Remove(_encryptionKeyName + keyName);
    }
}
