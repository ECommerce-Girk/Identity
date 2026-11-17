using Base.Database;
using Base.Infra;
using Core.Users;
using Microsoft.EntityFrameworkCore;

namespace Infra.Database;

public partial class IdentityDbContext(
    DbContextOptions<IdentityDbContext> options,
    IOutboxMessageRegistry messageRegistry) : OutboxDbContext(options, messageRegistry)
{
    public DbSet<User> Users => Set<User>();
}