using Base.Exceptions;
using Identity.Core.Abstractions;
using Identity.Core.Users;

namespace Identity.Core.Authentication;

public record LoginModel(string Email, string Password);

public class Login(
    ITokenProvider tokenProvider,
    IUserRepository userRepository,
    IPasswordHasher passwordHasher)
{
    public async Task<string> Execute(LoginModel model, CancellationToken ct)
    {
        var user = await userRepository.GetByEmail(model.Email, ct);

        if (user is null)
            throw new NotFoundEx("User not found");

        if (!passwordHasher.Verify(model.Password, user.PasswordHash))
            throw new NotAllowedEx("Invalid password");

        return tokenProvider.Create(user);
    }
}
