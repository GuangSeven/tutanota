namespace Tuta.Core.Crypto;

public interface IKeyStore
{
    Task StoreKeyAsync(string keyId, SymmetricKey key, CancellationToken cancellationToken);
    Task<SymmetricKey?> LoadKeyAsync(string keyId, CancellationToken cancellationToken);
}
