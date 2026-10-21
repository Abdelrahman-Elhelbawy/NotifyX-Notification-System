using NotifyX.Api.Entities.Enums;
namespace NotifyX.Api.Models.Notifications;

public record UpdateNotificationRequest(
    string? Title,
    string? Body,
    NotificationType? Type,
    int? Priority,
    DateTimeOffset? ScheduledAt,
    string? Metadata
);