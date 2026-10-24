using System;
using NotifyX.Api.Entities;

namespace NotifyX.Api.Repository.IdempotencyKeys;

public interface IIdempotencyKeysRepository
{
    // Checks whether the provided idempotency key already exists for the specified user
    // and returns the matching key record so duplicate requests can be safely identified.
    Task<IdempotencyKey?> GetExistingKeyAsync(Guid userId, string key, CancellationToken cancellationToken = default);
}