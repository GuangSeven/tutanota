using System.Text;
using Tuta.Core.Crypto;
using Tuta.Core.Storage;

namespace Tuta.Infrastructure.Storage;

public sealed class EncryptedEntityRepository<T>
{
    private readonly string _entityType;
    private readonly string _ownerGroupId;
    private readonly ILocalStore _store;
    private readonly IEntitySerializer _serializer;
    private readonly ICryptoService _crypto;
    private readonly IKeyStore _keyStore;

    public EncryptedEntityRepository(
        string entityType,
        string ownerGroupId,
        ILocalStore store,
        IEntitySerializer serializer,
        ICryptoService crypto,
        IKeyStore keyStore)
    {
        _entityType = entityType;
        _ownerGroupId = ownerGroupId;
        _store = store;
        _serializer = serializer;
        _crypto = crypto;
        _keyStore = keyStore;
    }

    public async Task SaveAsync(string entityId, string? listId, T entity, CancellationToken cancellationToken)
    {
        var key = await GetOrCreateKeyAsync(cancellationToken);
        var payload = _serializer.Serialize(entity);
        var aad = BuildAssociatedData(entityId, listId);
        var encrypted = _crypto.Encrypt(key, payload, aad);
        var envelope = new EntityEnvelope(_entityType, entityId, listId, _ownerGroupId, encrypted, key.Version);
        await _store.SaveAsync(envelope, cancellationToken);
    }

    public async Task<T?> LoadAsync(string entityId, CancellationToken cancellationToken)
    {
        var envelope = await _store.LoadAsync(_entityType, entityId, cancellationToken);
        if (envelope == null)
        {
            return default;
        }

        var key = await GetOrCreateKeyAsync(cancellationToken);
        var aad = BuildAssociatedData(envelope.EntityId, envelope.ListId);
        var payload = _crypto.Decrypt(key, envelope.Payload, aad);
        return _serializer.Deserialize<T>(payload);
    }

    public async Task<IReadOnlyList<T>> ListAsync(string listId, CancellationToken cancellationToken)
    {
        var envelopes = await _store.QueryListAsync(_entityType, listId, cancellationToken);
        var key = await GetOrCreateKeyAsync(cancellationToken);
        var result = new List<T>();
        foreach (var envelope in envelopes)
        {
            var aad = BuildAssociatedData(envelope.EntityId, envelope.ListId);
            var payload = _crypto.Decrypt(key, envelope.Payload, aad);
            result.Add(_serializer.Deserialize<T>(payload));
        }
        return result;
    }

    private async Task<SymmetricKey> GetOrCreateKeyAsync(CancellationToken cancellationToken)
    {
        var key = await _keyStore.LoadKeyAsync(_entityType, cancellationToken);
        if (key != null)
        {
            return key;
        }

        key = _crypto.GenerateKey();
        await _keyStore.StoreKeyAsync(_entityType, key, cancellationToken);
        return key;
    }

    private static byte[] BuildAssociatedData(string entityId, string? listId)
    {
        var buffer = $"{entityId}|{listId ?? string.Empty}";
        return Encoding.UTF8.GetBytes(buffer);
    }
}
