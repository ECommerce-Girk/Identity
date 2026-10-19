namespace Identity.Infra.Database;


public class Outbox
{
    public Guid Id { get; set; }
    public string EventType { get; set; } = null!;
    public string Payload { get; set; } = null!;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? PublishedAt { get; set; }
}