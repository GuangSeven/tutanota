using Tuta.Core.Domain;

namespace Tuta.Core.Services;

public interface IMailService
{
    Task<IReadOnlyList<MailSummary>> GetInboxAsync(string mailboxId, CancellationToken cancellationToken);
    Task<MailMessage?> GetMessageAsync(string mailboxId, string messageId, CancellationToken cancellationToken);
}
