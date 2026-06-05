using Tuta.Core.Crypto;

namespace Tuta.Core.Storage;

public sealed record StoredCredentials(
    byte[] Salt,
    int Iterations,
    byte[] PasswordHash,
    EncryptedPayload WrappedMasterKey,
    int KeyVersion
);
