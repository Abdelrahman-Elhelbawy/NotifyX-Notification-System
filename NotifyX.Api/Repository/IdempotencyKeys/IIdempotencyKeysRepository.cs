using System;
using NotifyX.Api.Entities;

namespace NotifyX.Api.Repository.IdempotencyKeys;

public interface IIdempotencyKeysRepository
{

    Task<IdempotencyKey?> GetExistingKeyAsync(Guid userId, string key, CancellationToken cancellationToken = default);
}