namespace NotifyX.Api.Entities;
public sealed class OutboxMessage
{
    public Guid Id { get; set; }
    public string EventType { get; set; } = null!;
    public string Payload { get; set; } = null!;
    public DateTimeOffset CreatedAtUtc { get; set; }
    public DateTimeOffset? ProcessedAtUtc { get; set; }
    public int Attempts { get; set; }
    public DateTimeOffset? NextAttemptAtUtc { get; set; }
    public string? LastError { get; set; }
    public DateTimeOffset? DeadLetteredAtUtc { get; set; }
}
