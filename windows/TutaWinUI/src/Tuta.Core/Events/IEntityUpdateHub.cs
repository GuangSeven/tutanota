namespace Tuta.Core.Events;

public interface IEntityUpdateHub
{
    event EventHandler<EntityUpdate>? EntityUpdated;
    Task PublishAsync(EntityUpdate update, CancellationToken cancellationToken);
}
