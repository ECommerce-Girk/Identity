using Base;

namespace Identity.Core.Users;

public record UserCreatedEvent(Guid UserId) : IDomainEvent;