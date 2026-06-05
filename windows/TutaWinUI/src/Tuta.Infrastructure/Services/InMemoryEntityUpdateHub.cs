using Tuta.Core.Events;

namespace Tuta.Infrastructure.Services;

public sealed class InMemoryEntityUpdateHub : IEntityUpdateHub
{
    public event EventHandler<EntityUpdate>? EntityUpdated;

    public Task PublishAsync(EntityUpdate update, CancellationToken cancellationToken)
    {
        EntityUpdated?.Invoke(this, update);
        return Task.CompletedTask;
    }
}
