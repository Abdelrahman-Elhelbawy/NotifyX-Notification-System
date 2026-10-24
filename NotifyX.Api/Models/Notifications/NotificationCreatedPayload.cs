
namespace NotifyX.Api.Models.Notifications;

public sealed record NotificationCreatedPayload(
    Guid NotificationId,
    IReadOnlyList<string> UserIds);
