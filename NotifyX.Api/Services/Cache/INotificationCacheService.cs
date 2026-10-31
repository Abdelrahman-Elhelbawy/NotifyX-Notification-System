namespace NotifyX.Api.Services.Cache;

public interface INotificationCacheService
{
    Task<string> GetVersionAsync(
        CancellationToken cancellationToken = default);

    Task InvalidateAsync(
        CancellationToken cancellationToken = default);
}