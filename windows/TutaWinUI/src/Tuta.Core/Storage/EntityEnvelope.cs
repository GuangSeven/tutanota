using Tuta.Core.Crypto;

namespace Tuta.Core.Storage;

public sealed record EntityEnvelope(
    string EntityType,
    string EntityId,
    string? ListId,
    string OwnerGroupId,
    EncryptedPayload Payload,
    int KeyVersion
);
