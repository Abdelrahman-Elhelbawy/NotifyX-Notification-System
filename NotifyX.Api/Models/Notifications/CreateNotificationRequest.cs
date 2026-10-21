using NotifyX.Api.Entities.Enums;
namespace NotifyX.Api.Models.Notifications;

public record CreateNotificationRequest(
    List<string> UserIds,
    string Title,
    string Body,
    NotificationType Type,
    int Priority,
    string? Metadata
);