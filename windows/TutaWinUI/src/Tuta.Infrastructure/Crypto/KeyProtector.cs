using Tuta.Core.Crypto;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.DataProtection;

namespace Tuta.Infrastructure.Crypto;

public sealed class KeyProtector : IKeyProtector
{
    private const string ProtectionDescriptor = "LOCAL=user";

    public async Task<byte[]> ProtectAsync(byte[] rawKey, CancellationToken cancellationToken)
    {
        var provider = new DataProtectionProvider(ProtectionDescriptor);
        var buffer = CryptographicBuffer.CreateFromByteArray(rawKey);
        var protectedBuffer = await provider.ProtectAsync(buffer).AsTask(cancellationToken);
        CryptographicBuffer.CopyToByteArray(protectedBuffer, out var result);
        return result;
    }

    public async Task<byte[]> UnprotectAsync(byte[] protectedKey, CancellationToken cancellationToken)
    {
        var provider = new DataProtectionProvider();
        var buffer = CryptographicBuffer.CreateFromByteArray(protectedKey);
        var unprotected = await provider.UnprotectAsync(buffer).AsTask(cancellationToken);
        CryptographicBuffer.CopyToByteArray(unprotected, out var result);
        return result;
    }
}
