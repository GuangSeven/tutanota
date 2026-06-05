using Tuta.Core.Domain;
using Tuta.Core.Services;
using Tuta.Infrastructure.Storage;

namespace Tuta.Infrastructure.Services;

public sealed class MockContactsService : IContactsService
{
    private const string ListId = "default";
    private readonly EncryptedEntityRepository<Contact> _repo;
    private readonly SemaphoreSlim _seedLock = new(1, 1);
    private bool _seeded;

    public MockContactsService(EncryptedEntityRepository<Contact> repo)
    {
        _repo = repo;
    }

    public async Task<IReadOnlyList<Contact>> GetAllAsync(CancellationToken cancellationToken)
    {
        await EnsureSeededAsync(cancellationToken);
        return await _repo.ListAsync(ListId, cancellationToken);
    }

    public async Task<IReadOnlyList<Contact>> SearchAsync(string query, CancellationToken cancellationToken)
    {
        var all = await GetAllAsync(cancellationToken);
        if (string.IsNullOrWhiteSpace(query))
        {
            return all;
        }

        return all.Where(c => c.DisplayName.Contains(query, StringComparison.OrdinalIgnoreCase)
                              || c.EmailAddresses.Any(e => e.Contains(query, StringComparison.OrdinalIgnoreCase)))
            .ToList();
    }

    private async Task EnsureSeededAsync(CancellationToken cancellationToken)
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

            foreach (var contact in SampleDataFactory.Contacts())
            {
                await _repo.SaveAsync(contact.Id, ListId, contact, cancellationToken);
            }

            _seeded = true;
        }
        finally
        {
            _seedLock.Release();
        }
    }
}
