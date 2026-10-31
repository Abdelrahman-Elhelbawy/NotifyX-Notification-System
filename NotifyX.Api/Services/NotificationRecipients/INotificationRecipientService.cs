using NotifyX.Api.Common.Results;
using NotifyX.Api.Models.NotificationRecipient;
using OneOf;

namespace NotifyX.Api.Services.NotificationRecipients;

public interface INotificationRecipientService
{
    /// <summary>
    /// Marks a notification as read for the specified user asynchronously.
    /// </summary>
    Task<OneOf<NotificationRecipientResponse, Error>> ReadAsync(
        Guid NotificationId,
        string userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks a notification as unread for the specified user asynchronously.
    /// </summary>
    Task<OneOf<NotificationRecipientResponse, Error>> UnReadAsync(
        Guid NotificationId,
        string userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the number of unread notifications for the specified user asynchronously.
    /// </summary>
    Task<OneOf<int, Error>> UnReadCountAsync(
        Guid NotificationId,
        string userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks all notifications as read for the specified user asynchronously.
    /// </summary>
    Task<OneOf<bool, Error>> ReadAllAsync(
        string userId,
        CancellationToken cancellationToken = default);
}
