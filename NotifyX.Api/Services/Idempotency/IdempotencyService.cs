using Microsoft.EntityFrameworkCore;
using NotifyX.Api.Data;
using NotifyX.Api.Entities;
using NotifyX.Api.Entities.Enums;

namespace NotifyX.Api.Services.Idempotency;

public sealed class IdempotencyService(
    ApplicationDbContext context) : IIdempotencyService
{
    private readonly ApplicationDbContext _context = context;

    /// <summary>
    /// Retrieves an existing idempotency record for the specified user and request key.
    /// </summary>
    /// <param name="userId">The identifier of the user associated with the request.</param>
    /// <param name="key">The idempotency key used to identify the request.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the operation to complete.</param>
    /// <returns>
    /// The existing idempotency record if one matches; otherwise, <see langword="null"/>.
    /// </returns>
    public async Task<IdempotencyKey?> GetExistingAsync(Guid userId, string key, CancellationToken cancellationToken = default)
    {
        return await _context.IdempotencyKeys
            .AsNoTracking()
            // .Include(x => x.Notification)
            .FirstOrDefaultAsync(
                x => x.UserId == userId && x.Key == key,
                cancellationToken);
    }

    
    /// <summary>
    /// Creates a new idempotency record for a request that has been accepted for processing.
    /// </summary>
    /// <param name="userId">The identifier of the user associated with the request.</param>
    /// <param name="key">The idempotency key used to identify the request.</param>
    /// <param name="requestHash">A hash of the request payload used to detect duplicates.</param>
    /// <param name="notificationId">The identifier of the notification associated with the request.</param>
    /// <returns>A newly created idempotency record.</returns>
    public IdempotencyKey CreateRecord(Guid userId, string key, string requestHash, Guid notificationId)
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