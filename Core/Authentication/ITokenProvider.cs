using Core.Users;

namespace Core.Authentication;

public interface ITokenProvider
{
    string Create(User user);
}