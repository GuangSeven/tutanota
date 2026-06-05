namespace Tuta.Core.Domain;

public sealed record MailMessage(
    string Id,
    string MailboxId,
    string From,
    IReadOnlyList<Recipient> To,
    IReadOnlyList<Recipient> Cc,
    IReadOnlyList<Recipient> Bcc,
    string Subject,
    string Body,
    DateTimeOffset SentAt,
    bool IsConfidential
);
