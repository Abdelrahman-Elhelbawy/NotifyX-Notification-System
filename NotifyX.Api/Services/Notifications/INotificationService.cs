using NotifyX.Api.Common.Results;
using NotifyX.Api.Entities;

namespace NotifyX.Api.Services.Notifications;

public interface INotificationService
{
    Task<Result<List<Notification>>> GetAllAsync(
        CancellationToken cancellationToken = default);
}