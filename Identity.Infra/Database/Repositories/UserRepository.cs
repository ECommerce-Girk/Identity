using Identity.Core.Users;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infra.Database.Repositories;

public class UserRepository(IdentityDbContext _dbContext): IUserRepository
{
    public async Task<User?> GetByEmail(string email, CancellationToken ct)
    {
        return await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email, ct);
    }
}