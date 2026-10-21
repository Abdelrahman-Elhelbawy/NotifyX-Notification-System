using NotifyX.Api.Common.Errors;
using NotifyX.Api.Common.Results;
using NotifyX.Api.Entities;
using NotifyX.Api.Models.Notifications;
using NotifyX.Api.Repositories.Notifications;

namespace NotifyX.Api.Services.Notifications;

public class NotificationService(
    INotificationRepository repository) : INotificationService
{
    private readonly INotificationRepository _repository = repository;

    public async Task<Result<NotificationResponse>> CreateAsync(
        CreateNotificationRequest request,
        CancellationToken cancellationToken = default!)
    {

        if (request.UserIds is null || request.UserIds.Count == 0)
        {
            return Result<NotificationResponse>.Failure(
                NotificationErrors.AtLeastOne);
        }

        var userIds = request.UserIds
            .Where(id => !string.IsNullOrWhiteSpace(id))
            .Distinct(StringComparer.Ordinal)
            .ToList();

        if (userIds.Count == 0)
        {
            return Result<NotificationResponse>.Failure(
                NotificationErrors.AtLeastOne);
        }

        var notification = new Notification
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Body = request.Body,
            Type = request.Type,
            Priority = request.Priority,
            CreatedAt = DateTimeOffset.UtcNow,
            Metadata = request.Metadata
        };

        foreach (var userId in userIds)
        {
            notification.Recipients.Add(new NotificationRecipient
            {
                NotificationId = notification.Id,
                UserId = userId,
                IsRead = false,
                ReadAtUtc = null
            });
        }

        await _repository.AddAsync(
            notification,
            cancellationToken);

        await _repository.SaveChangesAsync(cancellationToken);

        var response = new NotificationResponse(
            notification.Title,
            notification.Body,
            notification.Type,
            notification.Metadata);

        return Result<NotificationResponse>.Success(response);
    }

    public async Task<Result<List<Notification>>> GetAllAsync(
        CancellationToken cancellationToken = default!)
    {
        // TODO : Add Pagnation (skip)
        await _repository.GetAllAsync(cancellationToken);

        return Result<List<Notification>>.Success([]);
    }

    public async Task<Result<NotificationResponse>> GetAsync(Guid notificationId, CancellationToken cancellationToken = default)
    {
        var result = await _repository.GetAsync(notificationId, cancellationToken);

        if (result is null)
        {
            return Result<NotificationResponse>.Failure(
                NotificationErrors.NotFound);
        }

        var response = new NotificationResponse(
            result.Title,
            result.Body,
            result.Type,
            result.Metadata);

        return Result<NotificationResponse>.Success(response);
    }

    public async Task<Result<NotificationResponse>> UpdateAsync(
        Guid notificationId,
        UpdateNotificationRequest request,
        CancellationToken cancellationToken = default)
    {
        var notification = await _repository.GetAsync(
            notificationId,
            cancellationToken);

        if (notification is null)
        {
            return Result<NotificationResponse>.Failure(
                NotificationErrors.NotFound);
        }

        notification.Title = request.Title ?? notification.Title;
        notification.Body = request.Body ?? notification.Body;
        notification.Type = request.Type ?? notification.Type;
        notification.Priority = request.Priority ?? notification.Priority;
        notification.Metadata = request.Metadata ?? notification.Metadata;

        await _repository.UpdateAsync(
            notificationId,
            notification,
            cancellationToken);

        await _repository.SaveChangesAsync(cancellationToken);

        var response = new NotificationResponse(
            notification.Title,
            notification.Body,
            notification.Type,
            notification.Metadata);

        return Result<NotificationResponse>.Success(response);
    }
}