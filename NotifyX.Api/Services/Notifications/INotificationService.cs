using NotifyX.Api.Common.Results;
using NotifyX.Api.Entities;
using NotifyX.Api.Models.Notifications;

namespace NotifyX.Api.Services.Notifications;

public interface INotificationService
{
    Task<Result<NotificationResponse>> GetAsync(
        Guid notificationId,
        CancellationToken cancellationToken = default!);

    Task<Result<NotificationResponse>> CreateAsync(
        CreateNotificationRequest request,
        CancellationToken cancellationToken = default!);

    Task<Result<List<Notification>>> GetAllAsync(
        CancellationToken cancellationToken = default);
}