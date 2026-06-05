namespace Tuta.Core.Crypto;

public interface IMasterKeyProvider
{
    bool IsUnlocked { get; }
    SymmetricKey? CurrentKey { get; }
}
