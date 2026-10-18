using NotifyX.Api.Common.Results;

namespace NotifyX.Api.Common.Errors;

public static class NotificationErrors
{
    public static readonly Error NotFound =
        new(
            "Notification.NotFound",
            "Notification was not found",
            StatusCodes.Status404NotFound);

    public static readonly Error InvalidTitle =
        new(
            "Notification.InvalidTitle",
            "Notification title is required",
            StatusCodes.Status400BadRequest);

    public static readonly Error InvalidBody =
        new(
            "Notification.InvalidBody",
            "Notification body is required",
            StatusCodes.Status400BadRequest);

    public static readonly Error AlreadyRead =
        new(
            "Notification.AlreadyRead",
            "Notification has already been read",
            StatusCodes.Status409Conflict);
}
