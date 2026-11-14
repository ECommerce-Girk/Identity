using Base.Database;
using Identity.Core.Users;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infra.Database;

public partial class IdentityDbContext(
    DbContextOptions<IdentityDbContext> options,
    IMessageRegistry messageRegistry) : DbContextBase(options, messageRegistry)
{
    public DbSet<User> Users => Set<User>();
}