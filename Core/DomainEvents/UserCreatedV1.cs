using Base;

namespace Core.DomainEvents;

public record UserCreatedV1 : IDomainEvent
{
    public Guid Id { get; init; }
}