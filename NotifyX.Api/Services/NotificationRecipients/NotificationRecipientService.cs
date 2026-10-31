using NotifyX.Api.Common.Errors;
using NotifyX.Api.Common.Results;
using NotifyX.Api.Entities;
using NotifyX.Api.Models.NotificationRecipient;
using NotifyX.Api.Repository.NotificationRecipients;
using OneOf;

namespace NotifyX.Api.Services.NotificationRecipients;

public class NotificationRecipientService(
    INotificationRecipientRepository repository
    ) : INotificationRecipientService
{
    private readonly INotificationRecipientRepository _repository = repository;

    /// <summary>
    /// Marks a notification recipient's notification as read.
    /// </summary>
    /// <param name="NotificationId">The ID of the notification.</param>
    /// <param name="userId">The ID of the recipient.</param>
    /// <param name="cancellationToken">A token that can cancel the operation.</param>
    /// <returns>The recipient response, or an error if the recipient was not found or was already read.</returns>
    public async Task<OneOf<NotificationRecipientResponse, Error>> ReadAsync(Guid NotificationId, string userId, CancellationToken cancellationToken = default)
    {
        var recipient = await _repository.GetAsync(NotificationId, userId, cancellationToken);

        if (recipient is null)
            return NotificationRecipientErrors.NotFound;

        if (recipient.IsRead)
            return NotificationRecipientErrors.AlreadyRead;

        await _repository.ReadAsync(NotificationId, userId, cancellationToken);

        await _repository.SaveChangesAsync(cancellationToken);

        return BuildResponse(recipient);
    }

    /// <summary>
    /// Marks a notification recipient's notification as unread.
    /// </summary>
    /// <param name="NotificationId">The ID of the notification.</param>
    /// <param name="userId">The ID of the recipient.</param>
    /// <param name="cancellationToken">A token that can cancel the operation.</param>
    /// <returns>The recipient response, or an error if the recipient was not found or was already unread.</returns>
    public async Task<OneOf<NotificationRecipientResponse, Error>> UnReadAsync(Guid NotificationId, string userId, CancellationToken cancellationToken = default)
    {
        var recipient = await _repository.GetAsync(NotificationId, userId, cancellationToken);

        if (recipient is null)
            return NotificationRecipientErrors.NotFound;

        if (!recipient.IsRead)
            return NotificationRecipientErrors.AlreadyUnRead;


        await _repository.UnReadAsync(NotificationId, userId, cancellationToken);

        await _repository.SaveChangesAsync(cancellationToken);

        return BuildResponse(recipient);
    }

    /// <summary>
    /// Gets the unread notification count for a recipient.
    /// </summary>
    /// <param name="NotificationId">The ID of the notification.</param>
    /// <param name="userId">The ID of the recipient.</param>
    /// <param name="cancellationToken">A token that can cancel the operation.</param>
    /// <returns>The unread notification count, or an error.</returns>
    public async Task<OneOf<int, Error>> UnReadCountAsync(Guid NotificationId, string userId, CancellationToken cancellationToken = default)
    {
        var CountOfUnRead = await _repository.UnReadCountAsync(NotificationId, userId, cancellationToken);
        return CountOfUnRead;
    }

    /// <summary>
    /// Marks all notifications for a recipient as read.
    /// </summary>
    /// <param name="userId">The ID of the recipient.</param>
    /// <param name="cancellationToken">A token that can cancel the operation.</param>
    /// <returns>Whether the operation succeeded, or an error.</returns>
    public async Task<OneOf<bool, Error>> ReadAllAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await _repository.ReadAllAsync(userId, cancellationToken);
    }

    /// <summary>
    /// Builds a response model from a notification recipient entity.
    /// </summary>
    /// <param name="notificationRecipient">The notification recipient entity.</param>
    /// <returns>A response containing the recipient's notification and read status.</returns>
    private static NotificationRecipientResponse BuildResponse(NotificationRecipient notificationRecipient)
    {
        return new NotificationRecipientResponse(
            notificationRecipient.NotificationId,
            notificationRecipient.UserId,
            notificationRecipient.IsRead,
            notificationRecipient.ReadAtUtc
        );
    }
}