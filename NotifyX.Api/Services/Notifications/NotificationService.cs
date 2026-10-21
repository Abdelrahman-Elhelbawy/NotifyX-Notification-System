using NotifyX.Api.Common.Results;
using NotifyX.Api.Entities;
using NotifyX.Api.Repositories.Notifications;

namespace NotifyX.Api.Services.Notifications;

public class NotificationService(
    INotificationRepository repository) : INotificationService
{
    private readonly INotificationRepository _repository = repository;

    public async Task<Result<List<Notification>>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var result = await _repository.GetAllAsync(cancellationToken);

        if (result.IsFailure)
            return result;

        return Result<List<Notification>>.Success(result.Value!);
    }
}