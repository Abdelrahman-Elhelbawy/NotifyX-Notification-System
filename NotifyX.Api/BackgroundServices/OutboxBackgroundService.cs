using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using NotifyX.Api.Data;
using NotifyX.Api.Entities;
using NotifyX.Api.Models.Notifications;
using NotifyX.Api.Publishers;
using NotifyX.Api.Reliability;

namespace NotifyX.Api.BackgroundServices;

public sealed class OutboxBackgroundService(
    IServiceScopeFactory scopeFactory,
    ILogger<OutboxBackgroundService> logger)
    : BackgroundService
{
    private const int BatchSize = 20;
    private const int MaxAttempts = 5;

    private static readonly TimeSpan PollingInterval =
        TimeSpan.FromSeconds(5);

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(PollingInterval);
        try
        {
            do
            {
                try
                {
                    await ProcessBatchAsync(stoppingToken);
                }
                catch (OperationCanceledException)
                    when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception ex)
                {
                    logger.LogError(
                        ex,
                        "An unexpected error occurred while processing the outbox batch.");
                }
            }
            while (await timer.WaitForNextTickAsync(stoppingToken));
        }
        catch (OperationCanceledException)
            when (stoppingToken.IsCancellationRequested)
        {
            // Normal application shutdown.
        }
    }

    private async Task ProcessBatchAsync(
        CancellationToken cancellationToken)
    {
        using var scope = scopeFactory.CreateScope();

        var context = scope.ServiceProvider
            .GetRequiredService<ApplicationDbContext>();

        var publisher = scope.ServiceProvider
            .GetRequiredService<INotificationPublisher>();

        var now = DateTimeOffset.UtcNow;

        var messages = await context.OutboxMessages
            .Where(x =>
                x.ProcessedAtUtc == null &&
                x.DeadLetteredAtUtc == null &&
                (x.NextAttemptAtUtc == null ||
                x.NextAttemptAtUtc <= now))
            .OrderBy(x => x.CreatedAtUtc)
            .Take(BatchSize)
            .ToListAsync(cancellationToken);

        foreach (var message in messages)
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                await ProcessMessageAsync(
                    message,
                    context,
                    publisher,
                    cancellationToken);
            }
            catch (OperationCanceledException)
                when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "Failed to process outbox message {MessageId}.",
                    message.Id);

                message.ProcessedAtUtc = null;
                message.Attempts++;
                message.LastError = ex.Message;

                if (message.Attempts >= MaxAttempts)
                {
                    message.DeadLetteredAtUtc = DateTimeOffset.UtcNow;
                    message.NextAttemptAtUtc = null;
                }
                else
                {
                    message.NextAttemptAtUtc =
                        DateTimeOffset.UtcNow +
                        RetryPolicy.GetDelay(message.Attempts);
                }
                try
                {
                    await context.SaveChangesAsync(cancellationToken);
                }
                catch (Exception saveException)
                {
                    logger.LogError(
                        saveException,
                        "Could not save retry information for outbox message {MessageId}.",
                        message.Id);

                    // Do not terminate the service because retry-state
                    // persistence failed. The message remains in the database
                    // in its last successfully persisted state.
                    context.Entry(message).State = EntityState.Detached;
                }
            }
        }
    }

    private async Task ProcessMessageAsync(
        OutboxMessage message,
        ApplicationDbContext context,
        INotificationPublisher publisher,
        CancellationToken cancellationToken)
    {
        if (message.EventType != "NotificationCreated")
        {
            throw new InvalidOperationException(
                $"Unsupported outbox event type: {message.EventType}");
        }

        var payload =
            JsonSerializer.Deserialize<NotificationCreatedPayload>(
                message.Payload)
            ?? throw new InvalidOperationException(
                "Could not deserialize the outbox payload.");

        var notification = await context.Notifications
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.Id == payload.NotificationId,
                cancellationToken)
            ?? throw new InvalidOperationException(
                $"Notification {payload.NotificationId} was not found.");

        var response = new NotificationResponse(
            notification.Title,
            notification.Body,
            notification.Type,
            notification.Metadata);

        await publisher.PublishAsync(
            response,
            notification,
            payload.UserIds,
            cancellationToken);

        message.ProcessedAtUtc = DateTimeOffset.UtcNow;
        message.NextAttemptAtUtc = null;
        message.LastError = null;

        await context.SaveChangesAsync(cancellationToken);
    }
}