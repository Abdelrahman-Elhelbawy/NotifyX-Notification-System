using NotifyX.Api.Entities.Enums;
namespace NotifyX.Api.Models.Notifications;

public record NotificationResponse(
    Guid Id,
    string Title,
    string Body,
    NotificationType Type,
    string? Metadata
);