namespace Tuta.Core.Storage;

public interface ILocalStore
{
    Task InitializeAsync(UserContext context, CancellationToken cancellationToken);
    Task SaveAsync(EntityEnvelope envelope, CancellationToken cancellationToken);
    Task<EntityEnvelope?> LoadAsync(string entityType, string entityId, CancellationToken cancellationToken);
    Task<IReadOnlyList<EntityEnvelope>> QueryListAsync(string entityType, string listId, CancellationToken cancellationToken);
}
