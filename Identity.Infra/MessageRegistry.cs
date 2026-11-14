using Base.Database;
using Identity.Core.Users;

public class MessageRegistry : IMessageRegistry
{
    public IReadOnlyDictionary<string, Type> In => default!;
    
    public IReadOnlyDictionary<string, Type> Out => new Dictionary<string, Type>
    {
        ["identity.user.created.v1"] = typeof(UserCreatedV1)
    };
}