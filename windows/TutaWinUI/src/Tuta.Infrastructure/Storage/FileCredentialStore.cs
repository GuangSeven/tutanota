using System.Text.Json;
using Tuta.Core.Crypto;
using Tuta.Core.Storage;

namespace Tuta.Infrastructure.Storage;

public sealed class FileCredentialStore : ICredentialStore
{
    private readonly string _filePath;
    private readonly SemaphoreSlim _mutex = new(1, 1);

    public FileCredentialStore(string filePath)
    {
        _filePath = filePath;
    }

    public async Task<StoredCredentials?> LoadAsync(CancellationToken cancellationToken)
    {
        await _mutex.WaitAsync(cancellationToken);
        try
        {
            if (!File.Exists(_filePath))
            {
                return null;
            }

            await using var stream = File.OpenRead(_filePath);
            var dto = await JsonSerializer.DeserializeAsync<CredentialsDto>(stream, cancellationToken: cancellationToken);
            if (dto == null)
            {
                return null;
            }

            var payload = new EncryptedPayload(
                Convert.FromBase64String(dto.WrappedMasterKeyNonce),
                Convert.FromBase64String(dto.WrappedMasterKeyCiphertext),
                Convert.FromBase64String(dto.WrappedMasterKeyTag));

            return new StoredCredentials(
                Convert.FromBase64String(dto.Salt),
                dto.Iterations,
                Convert.FromBase64String(dto.PasswordHash),
                payload,
                dto.KeyVersion);
        }
        finally
        {
            _mutex.Release();
        }
    }

    public async Task SaveAsync(StoredCredentials credentials, CancellationToken cancellationToken)
    {
        await _mutex.WaitAsync(cancellationToken);
        try
        {
            var directory = Path.GetDirectoryName(_filePath);
            if (!string.IsNullOrWhiteSpace(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var dto = new CredentialsDto
            {
                Salt = Convert.ToBase64String(credentials.Salt),
                Iterations = credentials.Iterations,
                PasswordHash = Convert.ToBase64String(credentials.PasswordHash),
                WrappedMasterKeyNonce = Convert.ToBase64String(credentials.WrappedMasterKey.Nonce),
                WrappedMasterKeyCiphertext = Convert.ToBase64String(credentials.WrappedMasterKey.Ciphertext),
                WrappedMasterKeyTag = Convert.ToBase64String(credentials.WrappedMasterKey.Tag),
                KeyVersion = credentials.KeyVersion,
            };

            await using var stream = File.Create(_filePath);
            await JsonSerializer.SerializeAsync(stream, dto, cancellationToken: cancellationToken);
        }
        finally
        {
            _mutex.Release();
        }
    }

    private sealed record CredentialsDto
    {
        public string Salt { get; init; } = string.Empty;
        public int Iterations { get; init; }
        public string PasswordHash { get; init; } = string.Empty;
        public string WrappedMasterKeyNonce { get; init; } = string.Empty;
        public string WrappedMasterKeyCiphertext { get; init; } = string.Empty;
        public string WrappedMasterKeyTag { get; init; } = string.Empty;
        public int KeyVersion { get; init; }
    }
}
