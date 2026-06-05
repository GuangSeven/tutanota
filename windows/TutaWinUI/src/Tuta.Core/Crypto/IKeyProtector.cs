namespace Tuta.Core.Crypto;

public interface IKeyProtector
{
    Task<byte[]> ProtectAsync(byte[] rawKey, CancellationToken cancellationToken);
    Task<byte[]> UnprotectAsync(byte[] protectedKey, CancellationToken cancellationToken);
}
