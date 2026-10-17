using Identity.Infra.Database;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Tests;

[Collection(nameof(IntegrationTestCollection))]
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

[CollectionDefinition(nameof(IntegrationTestCollection))]
public sealed class IntegrationTestCollection : ICollectionFixture<CustomWebApplicationFactory>
{
};
