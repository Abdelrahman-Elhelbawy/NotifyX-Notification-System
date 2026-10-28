using System;
using NotifyX.Api.Entities;
namespace NotifyX.Api.Repository.NotificationRecipients;

public interface INotificationRecipientRepository
{
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

    Task<bool> UnReadAsync(
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
