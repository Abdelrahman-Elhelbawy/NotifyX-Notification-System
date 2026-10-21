using Microsoft.EntityFrameworkCore;
using NotifyX.Api.Common.Results;
using NotifyX.Api.Data;
using NotifyX.Api.Entities;
using NotifyX.Api.Repositories.Notifications;

namespace NotifyX.Api.Repository.Notifcations;

public class NotificationRepository(
    ApplicationDbContext context) : INotificationRepository
{
    private readonly ApplicationDbContext _context = context;

    public async Task<Result<List<Notification>>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var notifications = await _context.Notifications
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return Result<List<Notification>>.Success(notifications);
    }
}