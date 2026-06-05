namespace Tuta.Core.Events;

public enum EntityOperation
{
    Created,
    Updated,
    Deleted,
}

public sealed record EntityUpdate(
    string EntityType,
    string EntityId,
    EntityOperation Operation
);
