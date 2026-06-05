using Tuta.Core.Domain;
using Tuta.Core.Services;

namespace Tuta.Infrastructure.Services;

public sealed class MockMailboxService : IMailboxService
{
    public Task<IReadOnlyList<Mailbox>> GetMailboxesAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult(SampleDataFactory.Mailboxes());
    }
}
