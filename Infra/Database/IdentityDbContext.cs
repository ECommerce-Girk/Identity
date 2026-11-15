using Base.Database;
using Core.Users;
using Microsoft.EntityFrameworkCore;

namespace Infra.Database;

public partial class IdentityDbContext(
    DbContextOptions<IdentityDbContext> options,
    IMessageRegistry messageRegistry) : DbContextBase(options, messageRegistry)
{
    public DbSet<User> Users => Set<User>();
}