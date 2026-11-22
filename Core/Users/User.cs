using Base;
using Core.ValueObjects;

namespace Core.Users;

public class User : Entity
{
    public required string Name { get; set; }
    public required Email Email { get; set; }
    public required string PasswordHash { get; set; }
    public AccountStatusEnum AccountStatus { get; set; } = AccountStatusEnum.Active;
    public UserPoints Points { get; set; } = 0;
}