using Base.Database;
using Core.Users;
using Core.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Infra.Database;

public partial class IdentityDbContext(DbContextOptions<IdentityDbContext> options) : OutboxDbContext(options)
{
    public DbSet<User> Users => Set<User>();

    protected override void ConfigureValueObjects(ModelConfigurationBuilder configurationBuilder)
    {
        ValueObjectConverter.Text<Email, EmailConverter>(configurationBuilder);
        ValueObjectConverter.Text<Password, PasswordConverter>(configurationBuilder);
    }
}

internal sealed class PasswordConverter(): TextConverter<Password>(v => new Password(v));
internal sealed class EmailConverter(): TextConverter<Email>(v => new Email(v));