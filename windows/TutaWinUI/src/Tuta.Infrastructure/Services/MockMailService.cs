using Tuta.Core.Domain;
using Tuta.Core.Services;
using Tuta.Infrastructure.Storage;

namespace Tuta.Infrastructure.Services;

public sealed class MockMailService : IMailService
{
    private readonly EncryptedEntityRepository<MailMessage> _messageRepo;
    private readonly EncryptedEntityRepository<MailSummary> _summaryRepo;
    private readonly SemaphoreSlim _seedLock = new(1, 1);
    private bool _seeded;

    public MockMailService(
        EncryptedEntityRepository<MailMessage> messageRepo,
        EncryptedEntityRepository<MailSummary> summaryRepo)
    {
        _messageRepo = messageRepo;
        _summaryRepo = summaryRepo;
    }

    public async Task<IReadOnlyList<MailSummary>> GetInboxAsync(string mailboxId, CancellationToken cancellationToken)
    {
        await EnsureSeededAsync(mailboxId, cancellationToken);
        var list = await _summaryRepo.ListAsync(mailboxId, cancellationToken);
        return list.OrderByDescending(m => m.SentAt).ToList();
    }

    public async Task<MailMessage?> GetMessageAsync(string mailboxId, string messageId, CancellationToken cancellationToken)
    {
        await EnsureSeededAsync(mailboxId, cancellationToken);
        return await _messageRepo.LoadAsync(messageId, cancellationToken);
    }

    private async Task EnsureSeededAsync(string mailboxId, CancellationToken cancellationToken)
    {
        if (_seeded)
        {
            return;
        }

        await _seedLock.WaitAsync(cancellationToken);
        try
        {
            if (_seeded)
            {
                return;
            }

            var summaries = SampleDataFactory.MailSummaries(mailboxId);
            var messages = SampleDataFactory.MailMessages(mailboxId);

            foreach (var summary in summaries)
            {
                await _summaryRepo.SaveAsync(summary.Id, mailboxId, summary, cancellationToken);
            }

            foreach (var message in messages)
            {
                await _messageRepo.SaveAsync(message.Id, mailboxId, message, cancellationToken);
            }

            _seeded = true;
        }
        finally
        {
            _seedLock.Release();
        }
    }
}
