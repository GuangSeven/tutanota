using System.Text;
using System.Text.Json;
using Tuta.Core.Crypto;

namespace Tuta.Infrastructure.Crypto;

public sealed class MasterKeyProtector : IKeyProtector
{
    private static readonly byte[] AssociatedData = Encoding.UTF8.GetBytes("key-store");
    private readonly IMasterKeyProvider _masterKeyProvider;
    private readonly ICryptoService _crypto;

    public MasterKeyProtector(IMasterKeyProvider masterKeyProvider, ICryptoService crypto)
    {
        _masterKeyProvider = masterKeyProvider;
        _crypto = crypto;
    }

    public Task<byte[]> ProtectAsync(byte[] rawKey, CancellationToken cancellationToken)
    {
        var masterKey = GetMasterKey();
        var payload = _crypto.Encrypt(masterKey, rawKey, AssociatedData);
        var envelope = new KeyEnvelope(
            Convert.ToBase64String(payload.Nonce),
            Convert.ToBase64String(payload.Ciphertext),
            Convert.ToBase64String(payload.Tag));

        var bytes = JsonSerializer.SerializeToUtf8Bytes(envelope);
        return Task.FromResult(bytes);
    }

    public Task<byte[]> UnprotectAsync(byte[] protectedKey, CancellationToken cancellationToken)
    {
        var masterKey = GetMasterKey();
        var envelope = JsonSerializer.Deserialize<KeyEnvelope>(protectedKey)
                      ?? throw new InvalidOperationException("Invalid protected key payload.");

        var payload = new EncryptedPayload(
            Convert.FromBase64String(envelope.Nonce),
            Convert.FromBase64String(envelope.Ciphertext),
            Convert.FromBase64String(envelope.Tag));

        var raw = _crypto.Decrypt(masterKey, payload, AssociatedData);
        return Task.FromResult(raw);
    }

    private SymmetricKey GetMasterKey()
    {
        if (!_masterKeyProvider.IsUnlocked || _masterKeyProvider.CurrentKey == null)
        {
            throw new InvalidOperationException("Key store is locked.");
        }

        return _masterKeyProvider.CurrentKey;
    }

    private sealed record KeyEnvelope(string Nonce, string Ciphertext, string Tag);
}
