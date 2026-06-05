using System.Text.Json;
using Tuta.Core.Crypto;

namespace Tuta.Infrastructure.Crypto;

public sealed class FileKeyStore : IKeyStore
{
    private readonly string _filePath;
    private readonly IKeyProtector _protector;
    private readonly SemaphoreSlim _mutex = new(1, 1);

    public FileKeyStore(string filePath, IKeyProtector protector)
    {
        _filePath = filePath;
        _protector = protector;
    }

    public async Task StoreKeyAsync(string keyId, SymmetricKey key, CancellationToken cancellationToken)
    {
        await _mutex.WaitAsync(cancellationToken);
        try
        {
            var store = await ReadStoreAsync(cancellationToken);
            var protectedKey = await _protector.ProtectAsync(key.Material, cancellationToken);
            store[keyId] = new StoredKey(Convert.ToBase64String(protectedKey), key.Version);
            await WriteStoreAsync(store, cancellationToken);
        }
        finally
        {
            _mutex.Release();
        }
    }

    public async Task<SymmetricKey?> LoadKeyAsync(string keyId, CancellationToken cancellationToken)
    {
        await _mutex.WaitAsync(cancellationToken);
        try
        {
            var store = await ReadStoreAsync(cancellationToken);
            if (!store.TryGetValue(keyId, out var stored))
            {
                return null;
            }

            var protectedBytes = Convert.FromBase64String(stored.ProtectedKey);
            var raw = await _protector.UnprotectAsync(protectedBytes, cancellationToken);
            return new SymmetricKey(raw, stored.Version);
        }
        finally
        {
            _mutex.Release();
        }
    }

    private async Task<Dictionary<string, StoredKey>> ReadStoreAsync(CancellationToken cancellationToken)
    {
        if (!File.Exists(_filePath))
        {
            return new Dictionary<string, StoredKey>(StringComparer.OrdinalIgnoreCase);
        }

        await using var stream = File.OpenRead(_filePath);
        var store = await JsonSerializer.DeserializeAsync<Dictionary<string, StoredKey>>(stream, cancellationToken: cancellationToken);
        return store ?? new Dictionary<string, StoredKey>(StringComparer.OrdinalIgnoreCase);
    }

    private async Task WriteStoreAsync(Dictionary<string, StoredKey> store, CancellationToken cancellationToken)
    {
        var directory = Path.GetDirectoryName(_filePath);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        await using var stream = File.Create(_filePath);
        await JsonSerializer.SerializeAsync(stream, store, cancellationToken: cancellationToken);
    }

    private sealed record StoredKey(string ProtectedKey, int Version);
}
