namespace Tuta.Core.Domain;

public sealed record MailSummary(
    string Id,
    string MailboxId,
    string From,
    string Subject,
    DateTimeOffset SentAt,
    string Preview,
    bool IsUnread
);
