using NotifyX.Api.Entities;

namespace NotifyX.Api.Services.Idempotency;

public interface IIdempotencyService
{
    
    // Stores and resolves request idempotency keys per user to ensure the same
    // operation is not processed multiple times and to retain the original
    // notification reference for retries or duplicate submissions.
    Task<IdempotencyKey?> GetExistingAsync(Guid userId, string key, CancellationToken cancellationToken = default);

    // Creates a persisted idempotency record for a completed request so retries
    // can be recognized, the original notification can be resolved, and the same
    // payload is not processed again for the same user and key.
    IdempotencyKey CreateRecord(Guid userId, string key, string requestHash, Guid notificationId);
}