
using NotifyX.Api.Entities;
using NotifyX.Api.Models.Notifications;

namespace NotifyX.Api.Factories;

public sealed class NotificationFactory
{
    public Notification Create(CreateNotificationRequest request)
    {
        var notification = new Notification
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Body = request.Body,
            Type = request.Type,
            Priority = request.Priority,
            CreatedAt = DateTimeOffset.UtcNow,
            Metadata = request.Metadata
        };

        foreach (var recipientId in request.UserIds.Distinct())
        {
            notification.Recipients.Add(new NotificationRecipient
            {
                NotificationId = notification.Id,
                UserId = recipientId,
                IsRead = false,
                ReadAtUtc = null
            });
        }

        return notification;
    }
}
