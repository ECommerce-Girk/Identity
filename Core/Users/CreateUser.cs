using Base;
using Core.Abstractions;

namespace Core.Users;

public class CreateUserModel
{
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
}

public class CreateUser(
    IUoW _uow,
    IDomainEventCollector _collector,
    IPasswordHasher _passwordHasher,
    IUserRepository _userRepository)
{
    public async Task<User> Execute(CreateUserModel model, CancellationToken ct)
    {
        if(await _userRepository.GetByEmail(model.Email, ct) != null)
            throw new Exception("Email already exists");

        var user = new User()
        {
            Name = model.Name,
            Email = model.Email,
            PasswordHash = _passwordHasher.Hash(model.Password)
        };

        _uow.Add(user);
        _collector.Collect(new UserCreatedV1() { Id = user.Id });

        await _uow.SaveChanges(ct);
        return user;
    }
}