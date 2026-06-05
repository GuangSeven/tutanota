namespace Tuta.Core.Crypto;

public sealed record EncryptedPayload(byte[] Nonce, byte[] Ciphertext, byte[] Tag);
