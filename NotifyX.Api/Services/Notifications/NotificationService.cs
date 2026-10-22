using Microsoft.AspNetCore.SignalR;
using NotifyX.Api.Common.Errors;
using NotifyX.Api.Common.Results;
using NotifyX.Api.Entities;
using NotifyX.Api.Hubs;
using NotifyX.Api.Models.Notifications;
using NotifyX.Api.Repositories.Notifications;

namespace NotifyX.Api.Services.Notifications;

public class NotificationService(
    INotificationRepository repository,
    IHubContext<NotificationHub> hubContext,
    ILogger<NotificationService> logger
    ) : INotificationService
{
    private readonly INotificationRepository _repository = repository;
    private readonly IHubContext<NotificationHub> _hubContext = hubContext;
    private readonly ILogger<NotificationService> _logger = logger;

    /// Provides operations for creating, retrieving, updating, and deleting notifications.
    public async Task<Result<NotificationResponse>> CreateAsync(
        CreateNotificationRequest request,
        CancellationToken cancellationToken = default)
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

        var payload = new
        {
            notification.Id,
            response.Title,
            response.Body,
            response.Type,
            response.Metadata,
            notification.CreatedAt
        };

        foreach (var userId in userIds)
        {
            try
            {
                await _hubContext.Clients
                    .User(userId)
                    .SendAsync(
                        "ReceiveNotification",
                        payload,
                        cancellationToken);
            }
            catch (OperationCanceledException)
                when (cancellationToken.IsCancellationRequested)
            {
                // TODO : 
                break;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(
                    ex,
                    "SignalR delivery failed for notification {NotificationId} " +
                    "and user {UserId}.",
                    notification.Id,
                    userId);
            }
        }

        return Result<NotificationResponse>.Success(response);
    }

    /// Retrieves all notifications.
    public async Task<Result<List<NotificationResponse>>> GetAllAsync(
        CancellationToken cancellationToken = default!)
    {
        // TODO : Add Pagnation (skip)
        var result = await _repository.GetAllAsync(cancellationToken);

        var response = result
            .Select(notification => new NotificationResponse(
                notification.Title,
                notification.Body,
                notification.Type,
                notification.Metadata))
            .ToList();

        return Result<List<NotificationResponse>>.Success(response);
    }

    /// Retrieves a notification by its identifier.
    public async Task<Result<NotificationResponse>> GetAsync(
        Guid notificationId,
        CancellationToken cancellationToken = default)
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

    /// Updates a notification.
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

    /// Deletes a notification by its identifier.
    public async Task<Result<NotificationResponse>> DeleteAsync(Guid notificationId, CancellationToken cancellationToken = default)
    {
        var notification = await _repository.GetAsync(
            notificationId,
            cancellationToken);

        if (notification is null || notification.IsDeleted)
        {
            return Result<NotificationResponse>.Failure(
                NotificationErrors.NotFound);
        }

        await _repository.DeleteAsync(
            notificationId,
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