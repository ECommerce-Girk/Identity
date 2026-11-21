using Base;
using Core.Abstractions;
using Core.DomainEvents;
using Core.ValueObjects;

namespace Core.Users;

public class CreateUserModel
{
    public required string Name { get; set; }
    public required Email Email { get; set; }
    public required Password Password { get; set; }
}

public class CreateUser(
    IUoW _uow,
    ICollector _collector,
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