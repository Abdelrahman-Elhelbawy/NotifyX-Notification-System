using System;
using Microsoft.Extensions.Caching.Distributed;

namespace NotifyX.Api.Services.Cache;

public class NotificationCacheService(IDistributedCache cache) : INotificationCacheService
{
    private const string VersionKey = "notifications:version";

    private readonly IDistributedCache _cache = cache;

    public async Task<string> GetVersionAsync(
        CancellationToken cancellationToken = default)
    {
        var version = await _cache.GetStringAsync(
            VersionKey,
            cancellationToken);

        if (version is not null)
            return version;

        version = Guid.NewGuid().ToString("N");

        await _cache.SetStringAsync(
            VersionKey,
            version,
            cancellationToken);

        return version;
    }

    public async Task InvalidateAsync(
        CancellationToken cancellationToken = default)
    {
        var version = Guid.NewGuid().ToString("N");

        await _cache.SetStringAsync(
            VersionKey,
            version,
            cancellationToken);
    }
}