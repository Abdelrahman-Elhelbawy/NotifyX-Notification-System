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