namespace Tuta.Core.Domain;

public sealed record Mailbox(
    string Id,
    string DisplayName,
    string Address,
    bool IsPrimary
);
