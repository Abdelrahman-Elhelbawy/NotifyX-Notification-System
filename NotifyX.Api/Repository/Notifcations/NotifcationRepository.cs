using Microsoft.EntityFrameworkCore;
using NotifyX.Api.Common.Results;
using NotifyX.Api.Data;
using NotifyX.Api.Entities;
using NotifyX.Api.Models.Notifications;
using NotifyX.Api.Repositories.Notifications;

namespace NotifyX.Api.Repository.Notifcations;

public class NotificationRepository(
    ApplicationDbContext context) : INotificationRepository
{
    private readonly ApplicationDbContext _context = context;

    /// Creates a notification asynchronously.
    public async Task AddAsync(
    Notification notification,
    CancellationToken cancellationToken = default)
    {
        await _context.Notifications.AddAsync(
            notification,
            cancellationToken);
    }

    /// Retrieves all notifications asynchronously.
    public async Task GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        await _context.Notifications
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<Notification?> GetAsync(
     Guid notificationId,
     CancellationToken cancellationToken = default)
    {
        return await _context.Notifications
            .AsNoTracking()
            .FirstOrDefaultAsync(
                n => n.Id == notificationId,
                cancellationToken);
    }
    
    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }
}