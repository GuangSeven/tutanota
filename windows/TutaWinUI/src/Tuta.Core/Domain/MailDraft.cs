namespace Tuta.Core.Domain;

public sealed record MailDraft(
    string MailboxId,
    IReadOnlyList<Recipient> To,
    IReadOnlyList<Recipient> Cc,
    IReadOnlyList<Recipient> Bcc,
    string Subject,
    string Body,
    bool IsConfidential,
    DateTimeOffset? SendAt
);
