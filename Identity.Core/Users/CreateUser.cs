using Identity.Core.Abstractions;

namespace Identity.Core.Users;

public class CreateUserModel
{
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
}

public class CreateUser(
    IUnitOfWork uow,
    IPasswordHasher passwordHasher,
    IUserRepository userRepository)
{
    public async Task<User> Execute(CreateUserModel model, CancellationToken ct)
    {
        if(await userRepository.GetByEmail(model.Email, ct) != null)
            throw new Exception("Email already exists");

        var user = new User()
        {
            Name = model.Name,
            Email = model.Email,
            PasswordHash = passwordHasher.Hash(model.Password)
        };

        uow.Add(user);

        await uow.SaveChangesAsync(ct);
        return user;
    }
}