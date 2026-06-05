namespace Tuta.Core.Crypto;

public interface IPasswordKeyDeriver
{
    byte[] GenerateSalt(int lengthBytes = 16);
    byte[] DeriveKey(string password, byte[] salt, int iterations, byte domain, int lengthBytes = 32);
}
