using Microsoft.EntityFrameworkCore;
using NotifyX.Api.Data;
using NotifyX.Api.Entities;
using NotifyX.Api.Repository.IdempotencyKeys;

namespace NotifyX.Api.Repository.IdempotencyKeysRepository;

public class IdempotencyKeysRepository(ApplicationDbContext context) : IIdempotencyKeysRepository
{
    private readonly ApplicationDbContext _context = context;

    /// <summary>
    /// Retrieves an existing idempotency key for the specified user and key value.
    /// </summary>
    /// <param name="userId">The identifier of the user associated with the idempotency key.</param>
    /// <param name="key">The idempotency key value to look up.</param>
    /// <param name="cancellationToken">The cancellation token to observe while querying the database.</param>
    /// <returns>
    /// The matching <see cref="IdempotencyKey"/> if it exists; otherwise, <see langword="null"/>.
    /// </returns>
    public async Task<IdempotencyKey?> GetExistingKeyAsync(Guid userId, string key, CancellationToken cancellationToken = default)
    {
        return await _context.IdempotencyKeys
            .AsNoTracking()
            .Include(x => x.Notification)
            .FirstOrDefaultAsync(
                x => x.UserId == userId &&
                    x.Key == key,
                cancellationToken);
    }
}
