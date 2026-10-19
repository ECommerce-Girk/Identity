using Identity.Infra.Database;
using Microsoft.EntityFrameworkCore;

namespace Identity.Api;

public sealed class OutboxPublisher(IdentityDbContext dbContext) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var failures = 0;
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var unsentEvents = await dbContext.Outbox
                    .Where(e => e.PublishedAt == null)
                    .OrderBy(e => e.CreatedAt)
                    .ToListAsync(stoppingToken);

            }
            catch (Exception exception)
            {
                Console.WriteLine($"Error publishing events: {exception.Message}");
                failures++;
            }
        }
    }
}