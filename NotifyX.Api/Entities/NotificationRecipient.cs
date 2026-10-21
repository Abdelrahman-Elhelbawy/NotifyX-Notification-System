namespace NotifyX.Api.Entities;

public class NotificationRecipient
{
    public Guid NotificationId { get; set; }
    public string UserId { get; set; } = null!;
    public bool? IsRead { get; set; } = false ;
    public DateTime? ReadAtUtc { get; set; }

    // To link with Notification Table
    public Notification Notification { get; set; } = null!;
}