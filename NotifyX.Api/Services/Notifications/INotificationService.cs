using NotifyX.Api.Common.Results;
using NotifyX.Api.Models.Notifications;
using OneOf;

namespace NotifyX.Api.Services.Notifications;

public interface INotificationService
{
    /// <summary>
    /// Provides operations for creating, retrieving, updating, and deleting notifications.
    /// </summary>
    Task<OneOf<NotificationResponse , Error>> CreateAsync(
        CreateNotificationRequest request,
        string idempotencyKey,
        CancellationToken cancellationToken = default!);

    /// <summary>
    /// Retrieves all notifications.
    /// </summary>
    Task<OneOf< List<NotificationResponse> , Error>> GetAllAsync(
        CancellationToken cancellationToken = default!);

    /// <summary>
    /// Retrieves a notification by its identifier.
    /// </summary>
    Task<OneOf<NotificationResponse , Error>> GetAsync(
        Guid notificationId,
        CancellationToken cancellationToken = default!);

    /// <summary>
    /// Updates a notification.
    /// </summary>
    Task<OneOf<NotificationResponse , Error>> UpdateAsync(
        Guid notificationId,
        UpdateNotificationRequest request,
        CancellationToken cancellationToken = default!);

    /// <summary>
    /// Deletes a notification by its identifier.
    /// </summary>
    Task<OneOf<NotificationResponse , Error>> DeleteAsync(
        Guid notificationId,
        CancellationToken cancellationToken = default!);
}