using Infra.Database;
using Microsoft.Extensions.DependencyInjection;

namespace Tests;

public abstract class TestBase
{
    protected CustomWebApplicationFactory Factory { get; }
    protected HttpClient Client { get; }

    protected TestBase(CustomWebApplicationFactory factory)
    {
        Factory = factory;
        Client = factory.CreateClient();
    }

    protected IdentityDbContext GetDbContext()
    {
        var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();
        return dbContext;
    }
}
