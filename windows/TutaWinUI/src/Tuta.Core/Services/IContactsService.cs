using Tuta.Core.Domain;

namespace Tuta.Core.Services;

public interface IContactsService
{
    Task<IReadOnlyList<Contact>> GetAllAsync(CancellationToken cancellationToken);
    Task<IReadOnlyList<Contact>> SearchAsync(string query, CancellationToken cancellationToken);
}
