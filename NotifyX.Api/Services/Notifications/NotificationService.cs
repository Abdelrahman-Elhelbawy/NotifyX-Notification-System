using System.Data;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using NotifyX.Api.Common.Errors;
using NotifyX.Api.Common.Results;
using NotifyX.Api.Data;
using NotifyX.Api.Entities;
using NotifyX.Api.Entities.Enums;
using NotifyX.Api.Factories;
using NotifyX.Api.Models.Notifications;
using NotifyX.Api.Publishers;
using NotifyX.Api.Repositories.Notifications;
using NotifyX.Api.Repository.IdempotencyKeys;
using NotifyX.Api.Services.Idempotency;
using NotifyX.Api.Services.Outbox;
using OneOf;

namespace NotifyX.Api.Services.Notifications;

public class NotificationService(
    INotificationRepository repository,
    IIdempotencyKeysRepository idempotencyKeys,
    IIdempotencyService idempotencyService,
    IOutboxService outboxService,
    NotificationFactory notificationFactory,
    ApplicationDbContext context,
    INotificationPublisher publisher
    ) : INotificationService
{
    private readonly INotificationRepository _repository = repository;
    public readonly INotificationPublisher _publisher = publisher;
    public readonly IIdempotencyKeysRepository _idempotencyKeys = idempotencyKeys;
    public readonly ApplicationDbContext _context = context;
    public IIdempotencyService _idempotencyService = idempotencyService;

    public NotificationFactory _notificationFactory = notificationFactory;

    public IOutboxService _outboxService = outboxService;


    /// <summary>
    /// Validates and creates a notification, persists it with its recipients, and publishes it to the requested users.
    /// </summary>
    /// <param name="request">The notification details and recipient user IDs.</param>
    /// <param name="cancellationToken">A token that can cancel validation, persistence, or publishing.</param>
    /// <returns>
    /// A successful result containing the created notification, or a validation failure if the request is invalid.
    /// </returns>
    /// <summary>
    /// Validates and creates a notification, persists it with its recipients,
    /// and publishes it to the requested users.
    /// </summary>
    public async Task<OneOf<NotificationResponse, Error>> CreateAsync(CreateNotificationRequest request, string idempotencyKey, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(idempotencyKey) ||
            idempotencyKey.Length > 200)
        {
            return NotificationErrors.InvalidIdempotencyKey;
        }

        var userId = Guid.Parse(
            "11111111-1111-1111-1111-111111111111");

        var requestHash = IdempotencyHash.Compute(request);
        var strategy = _context.Database.CreateExecutionStrategy();

        var result = await strategy.ExecuteAsync(async () =>
        {
            await using var transaction =
                await _context.Database.BeginTransactionAsync(
                    IsolationLevel.Serializable,
                    cancellationToken);

            var existingKey =
                await _idempotencyService.GetExistingAsync(
                    userId,
                    idempotencyKey,
                    cancellationToken);

            if (existingKey is not null)
            {
                await transaction.RollbackAsync(cancellationToken);

                if (!string.Equals(
                        existingKey.RequestHash,
                        requestHash,
                        StringComparison.Ordinal))
                {
                    return (NotificationResponse?)null;
                }

                if (existingKey.Status == IdempotencyStatus.Completed &&
                    existingKey.Notification is not null)
                {
                    return BuildResponse(existingKey.Notification);
                }

                return (NotificationResponse?)null;
            }

            var notification = _notificationFactory.Create(request);

            var idempotencyRecord =
                _idempotencyService.CreateRecord(
                    userId,
                    idempotencyKey,
                    requestHash,
                    notification.Id);

            var payload = new NotificationCreatedPayload(
                notification.Id,
                request.UserIds.Distinct().ToArray());

            var outboxMessage = _outboxService.CreateRecord(
                "NotificationCreated",
                JsonSerializer.Serialize(payload));

            await _repository.AddAsync(
                notification,
                cancellationToken);

            await _context.IdempotencyKeys.AddAsync(
                idempotencyRecord,
                cancellationToken);

            await _context.OutboxMessages.AddAsync(
                outboxMessage,
                cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            return BuildResponse(notification);
        });

        if (result is null)
        {
            var existingKey =
                await _idempotencyService.GetExistingAsync(
                    userId,
                    idempotencyKey,
                    cancellationToken);

            if (existingKey is not null &&
                !string.Equals(
                    existingKey.RequestHash,
                    requestHash,
                    StringComparison.Ordinal))
            {
                return NotificationErrors.IdempotencyKeyConflict;
            }
            return NotificationErrors.IdempotencyRequestInProgress;
        }
        return result;
    }


    /// <summary>
    /// Retrieves all notifications currently available in the repository for the API consumers.
    /// </summary>
    /// <param name="cancellationToken">
    /// A token that can be used to cancel the retrieval operation if the request is aborted.
    /// </param>
    /// <returns>
    /// A successful result containing the list of notification payloads, or an empty list when no notifications exist.
    /// </returns>
    public async Task<OneOf<List<NotificationResponse>, Error>> GetAllAsync(CancellationToken cancellationToken = default!)
    {
        // TODO : Add Pagnation (skip)
        var result = await _repository.GetAllAsync(cancellationToken);

        var response = result
            .Select(notification => BuildResponse(notification))
            .ToList();

        return response;
    }

    /// <summary>
    /// Fetches a single notification by its unique identifier.
    /// </summary>
    /// <param name="notificationId">The globally unique id of the notification to retrieve.</param>
    /// <param name="cancellationToken">A token used to interrupt the query if the caller cancels the request.</param>
    /// <returns>
    /// A successful result with the requested notification payload, or a not-found result when no matching notification exists.
    /// </returns>
    public async Task<OneOf<NotificationResponse, Error>> GetAsync(Guid notificationId, CancellationToken cancellationToken = default)
    {
        var notification = await _repository.GetAsync(notificationId, cancellationToken);
        if (notification is null)
            return NotificationErrors.NotFound;

        return BuildResponse(notification);
    }

    /// <summary>
    /// Updates the editable properties of an existing notification without altering its identity.
    /// </summary>
    /// <param name="notificationId">The id of the notification that should be updated.</param>
    /// <param name="request">
    /// The updated field values. Any null property values are ignored so the current values remain unchanged.
    /// </param>
    /// <param name="cancellationToken">The cancellation token for the update operation.</param>
    /// <returns>
    /// A successful result with the updated notification payload, or a not-found result if the notification does not exist.
    /// </returns>
    public async Task<OneOf<NotificationResponse, Error>> UpdateAsync(Guid notificationId, UpdateNotificationRequest request, CancellationToken cancellationToken = default)
    {
        var notification = await _repository.GetAsync(
            notificationId,
            cancellationToken);

        if (notification is null)
            return NotificationErrors.NotFound;


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

        return BuildResponse(notification);
    }

    /// <summary>
    /// Marks a notification as deleted by removing it from the active collection through the repository layer.
    /// </summary>
    /// <param name="notificationId">The id of the notification to delete.</param>
    /// <param name="cancellationToken">The cancellation token for the delete operation.</param>
    /// <returns>
    /// A successful result with the deleted notification payload, or a not-found result when the notification cannot be found.
    /// </returns>
    public async Task<OneOf<NotificationResponse, Error>> DeleteAsync(Guid notificationId, CancellationToken cancellationToken = default)
    {
        var notification = await _repository.GetAsync(
            notificationId,
            cancellationToken);

        if (notification is null || notification.IsDeleted)
            return NotificationErrors.NotFound;


        await _repository.DeleteAsync(
            notificationId,
            cancellationToken);

        await _repository.SaveChangesAsync(cancellationToken);
        return BuildResponse(notification);
    }

    /// <summary>
    /// Maps a domain notification entity to the API response model used by clients.
    /// </summary>
    /// <param name="notification">The notification entity that needs to be converted to a transport-safe response.</param>
    /// <returns>A response object containing the notification's visible fields.</returns>
    private static NotificationResponse BuildResponse(Notification notification)
    {
        return new NotificationResponse(
            notification.Title,
            notification.Body,
            notification.Type,
            notification.Metadata);
    }
}