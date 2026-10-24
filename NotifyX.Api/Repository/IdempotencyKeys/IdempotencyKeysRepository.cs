using System;
using Microsoft.EntityFrameworkCore;
using NotifyX.Api.Data;
using NotifyX.Api.Entities;
using NotifyX.Api.Repository.IdempotencyKeys;

namespace NotifyX.Api.Repository.IdempotencyKeysRepository;

public class IdempotencyKeysRepository(ApplicationDbContext context) : IIdempotencyKeysRepository
{
    private readonly ApplicationDbContext _context = context;

    public async Task<IdempotencyKey?> GetExistingKeyAsync(
        Guid userId,
        string key,
        CancellationToken cancellationToken = default)
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
