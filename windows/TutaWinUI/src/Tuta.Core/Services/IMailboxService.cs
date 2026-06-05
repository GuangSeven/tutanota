using Tuta.Core.Domain;

namespace Tuta.Core.Services;

public interface IMailboxService
{
    Task<IReadOnlyList<Mailbox>> GetMailboxesAsync(CancellationToken cancellationToken);
}
