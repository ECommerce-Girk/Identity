using Base;

namespace Identity.Core.Users;

public record UserCreatedV1 : IDomainEvent
{
    public Guid Id { get; init; }
}