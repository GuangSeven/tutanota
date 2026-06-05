using Tuta.Core.Domain;
using Tuta.Core.Events;
using Tuta.Core.Services;
using Tuta.Infrastructure.Storage;

namespace Tuta.Infrastructure.Services;

public sealed class MockMailComposeService : IMailComposeService
{
    private readonly EncryptedEntityRepository<MailDraft> _draftRepo;
    private readonly EncryptedEntityRepository<MailMessage> _messageRepo;
    private readonly EncryptedEntityRepository<MailSummary> _summaryRepo;
    private readonly IEntityUpdateHub _updates;

    public MockMailComposeService(
        EncryptedEntityRepository<MailDraft> draftRepo,
        EncryptedEntityRepository<MailMessage> messageRepo,
        EncryptedEntityRepository<MailSummary> summaryRepo,
        IEntityUpdateHub updates)
    {
        _draftRepo = draftRepo;
        _messageRepo = messageRepo;
        _summaryRepo = summaryRepo;
        _updates = updates;
    }

    public async Task<string> SaveDraftAsync(MailDraft draft, CancellationToken cancellationToken)
    {
        var draftId = $"draft-{Guid.NewGuid():N}";
        await _draftRepo.SaveAsync(draftId, draft.MailboxId, draft, cancellationToken);
        await _updates.PublishAsync(new EntityUpdate("MailDraft", draftId, EntityOperation.Updated), cancellationToken);
        return draftId;
    }

    public async Task SendAsync(MailDraft draft, CancellationToken cancellationToken)
    {
        var messageId = $"mail-{Guid.NewGuid():N}";
        var message = new MailMessage(
            messageId,
            draft.MailboxId,
            "user@tuta.example",
            draft.To,
            draft.Cc,
            draft.Bcc,
            draft.Subject,
            draft.Body,
            draft.SendAt ?? DateTimeOffset.UtcNow,
            draft.IsConfidential);

        var summary = new MailSummary(
            messageId,
            draft.MailboxId,
            "user@tuta.example",
            draft.Subject,
            message.SentAt,
            draft.Body.Length > 80 ? draft.Body[..80] + "..." : draft.Body,
            true);

        await _messageRepo.SaveAsync(messageId, draft.MailboxId, message, cancellationToken);
        await _summaryRepo.SaveAsync(messageId, draft.MailboxId, summary, cancellationToken);
        await _updates.PublishAsync(new EntityUpdate("MailMessage", messageId, EntityOperation.Created), cancellationToken);
    }
}
