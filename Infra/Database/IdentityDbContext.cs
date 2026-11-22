using Base.Database;
using Core.Users;
using Microsoft.EntityFrameworkCore;

namespace Infra.Database;

public partial class IdentityDbContext(DbContextOptions<IdentityDbContext> options) : OutboxDbContext(options)
{
    public DbSet<User> Users => Set<User>();

    protected override void ConfigureValueObjects(ModelConfigurationBuilder c) => ValueObjectsConverters.Configure(c);
}
