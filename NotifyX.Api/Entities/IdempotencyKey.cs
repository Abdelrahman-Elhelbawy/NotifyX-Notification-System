using NotifyX.Api.Entities.Enums;

namespace NotifyX.Api.Entities;
public class IdempotencyKey
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Key { get; set; } = null!;
    public string RequestHash { get; set; } = null!;
    public IdempotencyStatus Status { get; set; }
    public Guid? NotificationId { get; set; }
    public Notification? Notification { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}