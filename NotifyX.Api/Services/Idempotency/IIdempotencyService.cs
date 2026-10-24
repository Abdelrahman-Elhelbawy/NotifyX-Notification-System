using NotifyX.Api.Entities;

namespace NotifyX.Api.Services.Idempotency;

public interface IIdempotencyService
{
    Task<IdempotencyKey?> GetExistingAsync(Guid userId, string key, CancellationToken cancellationToken = default);
    IdempotencyKey CreateRecord(Guid userId, string key, string requestHash, Guid notificationId);
}