using System.Security.Cryptography;
using Tuta.Core.Crypto;

namespace Tuta.Infrastructure.Crypto;

public sealed class PasswordKeyDeriver : IPasswordKeyDeriver
{
    public byte[] GenerateSalt(int lengthBytes = 16)
    {
        var salt = new byte[lengthBytes];
        RandomNumberGenerator.Fill(salt);
        return salt;
    }

    public byte[] DeriveKey(string password, byte[] salt, int iterations, byte domain, int lengthBytes = 32)
    {
        var domainSalt = new byte[salt.Length + 1];
        Buffer.BlockCopy(salt, 0, domainSalt, 0, salt.Length);
        domainSalt[^1] = domain;

        using var pbkdf2 = new Rfc2898DeriveBytes(password, domainSalt, iterations, HashAlgorithmName.SHA256);
        return pbkdf2.GetBytes(lengthBytes);
    }
}
