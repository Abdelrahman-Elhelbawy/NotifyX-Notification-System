using System;
using NotifyX.Api.Entities;
using NotifyX.Api.Models.Notifications;

namespace NotifyX.Api.Publishers;


public interface INotificationPublisher
{
    Task PublishAsync(
        NotificationResponse response,
        Notification notification,
        IReadOnlyCollection<string> userIds,
        CancellationToken cancellationToken = default!);
}
