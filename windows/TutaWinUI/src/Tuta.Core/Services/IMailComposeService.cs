using Tuta.Core.Domain;

namespace Tuta.Core.Services;

public interface IMailComposeService
{
    Task<string> SaveDraftAsync(MailDraft draft, CancellationToken cancellationToken);
    Task SendAsync(MailDraft draft, CancellationToken cancellationToken);
}
