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

    public static readonly Error AtLeastOne =
        new(
            "Notification.AtLeastOne",
            "At least one valid recipient is required.",
            StatusCodes.Status400BadRequest);

    public static readonly Error InvalidIdempotencyKey =
        new(
            "Notification.InvalidIdempotencyKey",
            "The Idempotency-Key header is required and must not exceed 200 characters.",
            StatusCodes.Status400BadRequest);

    public static readonly Error IdempotencyKeyConflict =
        new(
        "Notification.IdempotencyKeyConflict",
        "The Idempotency-Key was already used with a different request.",
        StatusCodes.Status409Conflict);

    public static readonly Error IdempotencyRequestInProgress =
        new(
            "Notification.IdempotencyRequestInProgress",
            "A request with this Idempotency-Key is already in progress.",
            StatusCodes.Status409Conflict);
}
