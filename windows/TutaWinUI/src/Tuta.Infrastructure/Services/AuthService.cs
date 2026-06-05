using System.Security.Cryptography;
using System.Text;
using Tuta.Core.Crypto;
using Tuta.Core.Services;
using Tuta.Core.Storage;

namespace Tuta.Infrastructure.Services;

public sealed class AuthService : IAuthService, IMasterKeyProvider
{
    private const int DefaultIterations = 120_000;
    private static readonly byte[] MasterKeyAssociatedData = Encoding.UTF8.GetBytes("master-key");
    private readonly ICredentialStore _credentialStore;
    private readonly IPasswordKeyDeriver _keyDeriver;
    private readonly ICryptoService _crypto;
    private SymmetricKey? _currentMasterKey;

    public AuthService(ICredentialStore credentialStore, IPasswordKeyDeriver keyDeriver, ICryptoService crypto)
    {
        _credentialStore = credentialStore;
        _keyDeriver = keyDeriver;
        _crypto = crypto;
    }

    public event EventHandler<AuthState>? AuthStateChanged;

    public bool IsUnlocked => _currentMasterKey != null;

    public SymmetricKey? CurrentKey => _currentMasterKey;

    public async Task<AuthState> GetStateAsync(CancellationToken cancellationToken)
    {
        var hasPassword = await IsPasswordSetAsync(cancellationToken);
        return new AuthState(IsUnlocked, hasPassword, "local-user");
    }

    public async Task<bool> IsPasswordSetAsync(CancellationToken cancellationToken)
    {
        var stored = await _credentialStore.LoadAsync(cancellationToken);
        return stored != null;
    }

    public async Task SetPasswordAsync(string password, CancellationToken cancellationToken)
    {
        var salt = _keyDeriver.GenerateSalt();
        var hashKey = _keyDeriver.DeriveKey(password, salt, DefaultIterations, 0x01);
        var wrapKey = _keyDeriver.DeriveKey(password, salt, DefaultIterations, 0x02);

        var masterKey = _crypto.GenerateKey();
        var wrapped = _crypto.Encrypt(new SymmetricKey(wrapKey, 1), masterKey.Material, MasterKeyAssociatedData);

        var stored = new StoredCredentials(salt, DefaultIterations, hashKey, wrapped, masterKey.Version);
        await _credentialStore.SaveAsync(stored, cancellationToken);

        _currentMasterKey = masterKey;
        AuthStateChanged?.Invoke(this, new AuthState(true, true, "local-user"));
    }

    public async Task<bool> SignInAsync(string password, CancellationToken cancellationToken)
    {
        var stored = await _credentialStore.LoadAsync(cancellationToken);
        if (stored == null)
        {
            return false;
        }

        var hashKey = _keyDeriver.DeriveKey(password, stored.Salt, stored.Iterations, 0x01);
        if (!CryptographicOperations.FixedTimeEquals(hashKey, stored.PasswordHash))
        {
            return false;
        }

        var wrapKey = _keyDeriver.DeriveKey(password, stored.Salt, stored.Iterations, 0x02);
        var master = _crypto.Decrypt(new SymmetricKey(wrapKey, stored.KeyVersion), stored.WrappedMasterKey, MasterKeyAssociatedData);
        _currentMasterKey = new SymmetricKey(master, stored.KeyVersion);

        AuthStateChanged?.Invoke(this, new AuthState(true, true, "local-user"));
        return true;
    }

    public Task SignOutAsync(CancellationToken cancellationToken)
    {
        if (_currentMasterKey != null)
        {
            CryptographicOperations.ZeroMemory(_currentMasterKey.Material);
        }

        _currentMasterKey = null;
        AuthStateChanged?.Invoke(this, new AuthState(false, true, "local-user"));
        return Task.CompletedTask;
    }
}
