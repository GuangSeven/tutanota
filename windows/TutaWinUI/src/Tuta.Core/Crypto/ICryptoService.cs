namespace Tuta.Core.Crypto;

public interface ICryptoService
{
    SymmetricKey GenerateKey(int lengthBytes = 32);
    EncryptedPayload Encrypt(SymmetricKey key, ReadOnlySpan<byte> plaintext, ReadOnlySpan<byte> associatedData);
    byte[] Decrypt(SymmetricKey key, EncryptedPayload payload, ReadOnlySpan<byte> associatedData);
    SymmetricKey DeriveKey(SymmetricKey key, string salt, KeyDerivationDomain domain, int lengthBytes = 32);
    byte[] ComputeHmac(SymmetricKey key, ReadOnlySpan<byte> data);
}
