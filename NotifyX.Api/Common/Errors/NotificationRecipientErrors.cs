using NotifyX.Api.Common.Results;

namespace NotifyX.Api.Common.Errors;

public static class NotificationRecipientErrors
{
    public static readonly Error NotFound =
        new(
            "NotificationRecipient.NotFound",
            "NotificationRecipient was not found",
            StatusCodes.Status404NotFound);

    public static readonly Error AlreadyRead =
        new(
            "NotificationRecipient.AlreadyRead",
            "NotificationRecipient already read",
            StatusCodes.Status400BadRequest);

    public static readonly Error AlreadyUnRead =
        new(
            "NotificationRecipient.AlreadyUnRead",
            "NotificationRecipient already unread",
            StatusCodes.Status400BadRequest);
}
