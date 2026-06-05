namespace Tuta.Core.Services;

public sealed record AuthState(
    bool IsUnlocked,
    bool IsPasswordSet,
    string? UserId
);
