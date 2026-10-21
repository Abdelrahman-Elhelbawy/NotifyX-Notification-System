using NotifyX.Api.Common.Results;
using NotifyX.Api.Entities;
using NotifyX.Api.Models.Notifications;

namespace NotifyX.Api.Repositories.Notifications;

public interface INotificationRepository
{

    /// <summary>
    /// Creates a notification asynchronously.
    /// </summary>
    Task AddAsync(
        Notification notification,
        CancellationToken cancellationToken = default!);

    /// <summary>
    /// Retrieves a notification by its identifier asynchronously.
    /// </summary>
    Task<Notification?> GetAsync(
        Guid notificationId,
        CancellationToken cancellationToken = default!
    );

    /// <summary>
    /// Retrieves all notifications asynchronously.
    /// </summary>
    Task GetAllAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Saves pending notification changes asynchronously.
    /// </summary>
    Task SaveChangesAsync(
        CancellationToken cancellationToken = default!
    );
}