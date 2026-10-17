using Identity.Core.Users;

namespace Identity.Core.Authentication;

public interface ITokenProvider
{
    string Create(User user);
}