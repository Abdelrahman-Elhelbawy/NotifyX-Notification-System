using NotifyX.Api.Common.Results;
using NotifyX.Api.Entities;

namespace NotifyX.Api.Repositories.Notifications;

public interface INotificationRepository
{
    Task<Result<List<Notification>>> GetAllAsync(
        CancellationToken cancellationToken = default);
}