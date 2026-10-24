using Microsoft.EntityFrameworkCore;
using NotifyX.Api.Data;
using NotifyX.Api.Entities;
using NotifyX.Api.Entities.Enums;

namespace NotifyX.Api.Services.Idempotency;

public sealed class IdempotencyService(
    ApplicationDbContext context) : IIdempotencyService
{
    public async Task<IdempotencyKey?> GetExistingAsync(
        Guid userId,
        string key,
        CancellationToken cancellationToken = default)
    {
        return await context.IdempotencyKeys
            .AsNoTracking()
            .Include(x => x.Notification)
            .FirstOrDefaultAsync(
                x => x.UserId == userId && x.Key == key,
                cancellationToken);
    }

    public IdempotencyKey CreateRecord(
        Guid userId,
        string key,
        string requestHash,
        Guid notificationId)
    {
        return new IdempotencyKey
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Key = key,
            RequestHash = requestHash,
            Status = IdempotencyStatus.Completed,
            NotificationId = notificationId,
            CreatedAtUtc = DateTime.UtcNow
        };
    }
}