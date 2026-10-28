namespace NotifyX.Api.Models.NotificationRecipient;

public record NotificationRecipientResponse(
    Guid notificationId,
    string userId,
    bool IsRead,
    DateTime? readAtUtc);