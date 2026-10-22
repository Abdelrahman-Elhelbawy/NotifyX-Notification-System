using NotifyX.Api.Common.Results;
using NotifyX.Api.Entities;
using NotifyX.Api.Models.Notifications;

namespace NotifyX.Api.Services.Notifications;

public interface INotificationService
{
    /// <summary>
    /// Provides operations for creating, retrieving, updating, and deleting notifications.
    /// </summary>
    Task<Result<NotificationResponse>> CreateAsync(
        CreateNotificationRequest request,
        CancellationToken cancellationToken = default!);

    /// <summary>
    /// Retrieves all notifications.
    /// </summary>
    Task<Result<List<NotificationResponse>>> GetAllAsync(
        CancellationToken cancellationToken = default!);

    /// <summary>
    /// Retrieves a notification by its identifier.
    /// </summary>
    Task<Result<NotificationResponse>> GetAsync(
        Guid notificationId,
        CancellationToken cancellationToken = default!);

    /// <summary>
    /// Updates a notification.
    /// </summary>
    Task<Result<NotificationResponse>> UpdateAsync(
        Guid notificationId,
        UpdateNotificationRequest request,
        CancellationToken cancellationToken = default!);

    /// <summary>
    /// Deletes a notification by its identifier.
    /// </summary>
    Task<Result<NotificationResponse>> DeleteAsync(
        Guid notificationId,
        CancellationToken cancellationToken = default!);
}