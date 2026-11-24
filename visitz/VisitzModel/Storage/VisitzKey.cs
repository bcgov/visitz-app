using System.Collections.Concurrent;
using System.Security.Cryptography;

namespace VisitzModel.Storage;

public static class VisitzKey
{
    private static readonly int _defaultKeySize = 64;

    private static readonly string _encryptionKeyName = "visitz.encryption.key.";

    private static readonly SemaphoreSlim _semaphore = new(1);

    private static readonly ConcurrentDictionary<string, byte[]> s_keysCache = [];

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

        // TS: Ran into major performance issues that required us to cache keys in memory. This has turned into a hot
        // path--with enough fast hits we "saturated" SecureStorage.Default.GetAsync, causing it to freeze in
        // external unmanaged code. Since the function never returns, this app has no way to recover from it. So we
        // need to pre-empt the issue with caching.
        //
        // I think the more ideal solution would be to hold one single Realm instance per realm DB file for the
        // lifetime of the app instead of caching these keys, but... the app is built around Realm's recommended
        // "make and dispose new instances when you need them". It would be a lot of work to rearchitect the app
        // for this fix. It will probably be necessary eventually, but for now, this works.
        //
        // Other notes, if we decide to keep this cache going forward, we could probably rework the code so the
        // cache is checked before entering the semaphore, since it's a ConcurrentDictionary already.

        if (s_keysCache.TryGetValue(namespacedKey, out byte[]? cachedKey) && cachedKey != null)
            return cachedKey;
        else if (await SecureStorage.Default.GetAsync(namespacedKey) is string encodedKey)
        {
            byte[] key = Convert.FromBase64String(encodedKey);
            s_keysCache.TryAdd(namespacedKey, key);
            return key;
        }

        return null;
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
