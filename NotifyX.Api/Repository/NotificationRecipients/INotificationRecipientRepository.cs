using System;
using NotifyX.Api.Entities;
namespace NotifyX.Api.Repository.NotificationRecipients;

public interface INotificationRecipientRepository
{
    /// <summary>
    /// Get a notification recipient asynchronously.
    /// </summary>
    Task<NotificationRecipient?> GetAsync(
        Guid notificationId,
        string userId,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Updates the read status of a notification asynchronously.
    /// </summary>
    Task<bool> ReadAsync(
        Guid notificationId,
        string userId,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Updates the unread status of a notification asynchronously.
    /// </summary>
    Task<bool> UnReadAsync(
        Guid notificationId,
        string userId,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Gets a unread count of a notification recipient asynchronously.
    /// </summary>
    Task<int> UnReadCountAsync(
        Guid notificationId,
        string userId,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Saves pending notification changes asynchronously.
    /// </summary>
    Task SaveChangesAsync(
        CancellationToken cancellationToken = default!
    );
}
