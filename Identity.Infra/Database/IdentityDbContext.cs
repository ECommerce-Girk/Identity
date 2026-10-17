using Identity.Core.Users;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infra.Database;

public partial class IdentityDbContext(DbContextOptions<IdentityDbContext> options): DbContext(options)
{
    public DbSet<User> Users => Set<User>();
}