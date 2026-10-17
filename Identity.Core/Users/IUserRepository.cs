namespace Identity.Core.Users;

public interface IUserRepository
{
   Task<User?> GetByEmail(string email, CancellationToken ct);
}