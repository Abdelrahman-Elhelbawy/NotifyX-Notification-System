using NotifyX.Api.Entities;

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
    /// Retrieves all notifications asynchronously.
    /// </summary>
    Task<List<Notification>> GetAllAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a notification by its identifier asynchronously.
    /// </summary>
    Task<Notification?> GetAsync(
        Guid notificationId,
        CancellationToken cancellationToken = default!
    );

    /// <summary>
    /// Updates an existing notification asynchronously.
    /// </summary>
    Task<bool> UpdateAsync(
        Guid notificationId,
        Notification notification,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a notification by its identifier asynchronously.
    /// </summary>
    Task<bool> DeleteAsync(
        Guid notificationId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Saves pending notification changes asynchronously.
    /// </summary>
    Task SaveChangesAsync(
        CancellationToken cancellationToken = default!
    );
}