using NotifyX.Api.Entities.Enums;

namespace NotifyX.Api.Entities;

public class Notification
{
    public Guid Id { get; set; }

    // Data for the notification, such as the title, body, type, and priority.
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public NotificationType Type { get; set; } = NotificationType.General;
    public int Priority { get; set; }

    /*
        -- CreatedAt is the timestamp when the notification was created
        -- ScheduledAt is an optional timestamp for when the notification is scheduled to be sent.
    */
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? ScheduledAt { get; set; }

    // To Apply a Soft Deleted , A Default Value if False
    public bool IsDeleted { get; set; } = false;

    // Metadata is a JSON string that can hold additional information about the notification.
    public string? Metadata { get; set; }

    // Ref to NotificationRecipient Table
    public ICollection<NotificationRecipient> Recipients { get; set; }
    = new List<NotificationRecipient>();

    // Ref to IdempotencyKey Table
     public ICollection<IdempotencyKey> IdempotencyKeys { get; set; }
        = new List<IdempotencyKey>();
}