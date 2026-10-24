
using Microsoft.AspNetCore.SignalR;
using NotifyX.Api.Entities;
using NotifyX.Api.Hubs;
using NotifyX.Api.Models.Notifications;
using NotifyX.Api.Publishers;

public sealed class SignalRNotificationPublisher
    : INotificationPublisher
{
    private readonly IHubContext<NotificationHub> _hubContext;
    private readonly ILogger<SignalRNotificationPublisher> _logger;

    public SignalRNotificationPublisher(
        IHubContext<NotificationHub> hubContext,
        ILogger<SignalRNotificationPublisher> logger)
    {
        _hubContext = hubContext;
        _logger = logger;
    }
    public async Task PublishAsync(
        NotificationResponse response,
        Notification notification,
        IReadOnlyCollection<string> userIds,
        CancellationToken cancellationToken = default)
    {
        var payload = new
        {
            notification.Id,
            response.Title,
            response.Body,
            response.Type,
            response.Metadata,
            notification.CreatedAt
        };

        foreach (var userId in userIds)
        {
            try
            {
                await _hubContext.Clients
                    .User(userId)
                    .SendAsync(
                        "ReceiveNotification",
                        payload,
                        cancellationToken);
            }
            catch (OperationCanceledException)
                when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(
                    ex,
                    "SignalR delivery failed for notification {NotificationId} and user {UserId}.",
                    notification.Id,
                    userId);
            }
        }
    }
}