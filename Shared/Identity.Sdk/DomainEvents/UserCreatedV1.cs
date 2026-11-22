using Base;

namespace Identity.Sdk;

public record UserCreatedV1 : IDomainEvent
{
    public Guid Id { get; init; }
}