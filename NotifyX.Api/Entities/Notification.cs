using NotifyX.Api.Entities.Enums;

namespace NotifyX.Api.Entities;

public class Notification
{
    public Guid Id { get; set; }

    /**
        -- UserId is an optional foreign key to the user who will receive the notification.
        -- If UserId is null, the notification is considered a broadcast notification.
        -- User ID Property , This can be utilized by those who perform an integration.
    */
    public Guid? UserId { get; set; }

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

    public bool IsRead { get; set; } = false;
    public DateTimeOffset? ReadAtUtc { get; set; }

    // Metadata is a JSON string that can hold additional information about the notification.
    public string? Metadata { get; set; } 
}