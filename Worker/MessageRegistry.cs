using Base.Message;
using Core.DomainEvents;

public class MessageRegistry : IOutboxMessageRegistry
{
    public IReadOnlyDictionary<string, Type> Out => new Dictionary<string, Type>
    {
        ["identity.user.created.v1"] = typeof(UserCreatedV1)
    };
}