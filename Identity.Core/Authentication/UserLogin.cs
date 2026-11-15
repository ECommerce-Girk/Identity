using Base.Exceptions;
using Identity.Core.Abstractions;
using Identity.Core.Users;

namespace Identity.Core.Authentication;

public record UserLoginModel(string Email, string Password);

public class UserLogin(
    ITokenProvider _tokenProvider,
    IUserRepository _userRepository,
    IPasswordHasher _passwordHasher)
{
    public async Task<string> Execute(UserLoginModel model, CancellationToken ct)
    {
        var user = await _userRepository.GetByEmail(model.Email, ct);

        if (user is null)
            throw new NotFoundEx("User not found");

        if (!_passwordHasher.Verify(model.Password, user.PasswordHash))
            throw new NotAllowedEx("Invalid password");

        return _tokenProvider.Create(user);
    }
}
