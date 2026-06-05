using System.Security.Cryptography;
using System.Text;
using Tuta.Core.Crypto;

namespace Tuta.Infrastructure.Crypto;

public sealed class AesGcmCryptoService : ICryptoService
{
    private const int NonceSize = 12;
    private const int TagSize = 16;

    public SymmetricKey GenerateKey(int lengthBytes = 32)
    {
        var material = new byte[lengthBytes];
        RandomNumberGenerator.Fill(material);
        return new SymmetricKey(material, 1);
    }

    public EncryptedPayload Encrypt(SymmetricKey key, ReadOnlySpan<byte> plaintext, ReadOnlySpan<byte> associatedData)
    {
        var nonce = new byte[NonceSize];
        RandomNumberGenerator.Fill(nonce);

        var ciphertext = new byte[plaintext.Length];
        var tag = new byte[TagSize];

        using var aes = new AesGcm(key.Material, TagSize);
        aes.Encrypt(nonce, plaintext, ciphertext, tag, associatedData);

        return new EncryptedPayload(nonce, ciphertext, tag);
    }

    public byte[] Decrypt(SymmetricKey key, EncryptedPayload payload, ReadOnlySpan<byte> associatedData)
    {
        var plaintext = new byte[payload.Ciphertext.Length];
        using var aes = new AesGcm(key.Material, TagSize);
        aes.Decrypt(payload.Nonce, payload.Ciphertext, payload.Tag, plaintext, associatedData);
        return plaintext;
    }

    public SymmetricKey DeriveKey(SymmetricKey key, string salt, KeyDerivationDomain domain, int lengthBytes = 32)
    {
        var prk = HmacSha256(Encoding.UTF8.GetBytes(salt), key.Material);
        var info = Encoding.UTF8.GetBytes(domain.ToString());
        var okm = HkdfExpand(prk, info, lengthBytes);
        return new SymmetricKey(okm, key.Version);
    }

    public byte[] ComputeHmac(SymmetricKey key, ReadOnlySpan<byte> data)
    {
        return HmacSha256(key.Material, data.ToArray());
    }

    private static byte[] HmacSha256(byte[] key, byte[] data)
    {
        using var hmac = new HMACSHA256(key);
        return hmac.ComputeHash(data);
    }

    private static byte[] HkdfExpand(byte[] prk, byte[] info, int length)
    {
        var hashLen = 32;
        var n = (int)Math.Ceiling(length / (double)hashLen);
        var okm = new byte[length];
        var previous = Array.Empty<byte>();
        var offset = 0;

        for (var i = 1; i <= n; i++)
        {
            using var hmac = new HMACSHA256(prk);
            var buffer = new byte[previous.Length + info.Length + 1];
            Buffer.BlockCopy(previous, 0, buffer, 0, previous.Length);
            Buffer.BlockCopy(info, 0, buffer, previous.Length, info.Length);
            buffer[buffer.Length - 1] = (byte)i;
            previous = hmac.ComputeHash(buffer);

            var toCopy = Math.Min(hashLen, length - offset);
            Buffer.BlockCopy(previous, 0, okm, offset, toCopy);
            offset += toCopy;
        }

        return okm;
    }
}
