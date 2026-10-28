using NotifyX.Api.Common.Results;
using NotifyX.Api.Models.NotificationRecipient;
using OneOf;

namespace NotifyX.Api.Services.NotificationRecipients;

public interface INotificationRecipientService
{
    Task<OneOf<NotificationRecipientResponse, Error>> ReadAsync(
        Guid NotificationId,
        string userId,
        CancellationToken cancellationToken = default);

    Task<OneOf<NotificationRecipientResponse, Error>> UnReadAsync(
        Guid NotificationId,
        string userId,
        CancellationToken cancellationToken = default);
}
