using Microsoft.EntityFrameworkCore;
using NotifyX.Api.Data;
using NotifyX.Api.Entities;

namespace NotifyX.Api.Repository.NotificationRecipients;

public class NotificationRecipientRepository(ApplicationDbContext context) : INotificationRecipientRepository
{
    public ApplicationDbContext _context = context;

    public async Task<bool> ReadAsync(Guid notificationId, string userId, CancellationToken cancellationToken = default)
    {
        var recipient = await _context.NotificationRecipients
            .FirstOrDefaultAsync(
                x => x.NotificationId == notificationId
                        && x.UserId == userId,
                cancellationToken);

        if (recipient is null)
            return false;

        if (recipient.IsRead)
            return true;

        recipient.IsRead = true;
        recipient.ReadAtUtc = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> UnReadAsync(Guid notificationId, string userId, CancellationToken cancellationToken = default)
    {
        var recipient = await _context.NotificationRecipients
            .FirstOrDefaultAsync(
                x => x.NotificationId == notificationId
                        && x.UserId == userId,
                cancellationToken);

        if (recipient is null)
            return false;

        if (!recipient.IsRead)
            return true;

        recipient.IsRead = false;
        recipient.ReadAtUtc = null;

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<NotificationRecipient?> GetAsync(Guid notificationId, string userId, CancellationToken cancellationToken = default)
    {
        return await _context.NotificationRecipients
            .AsNoTracking()
            .FirstOrDefaultAsync(
                nr => nr.NotificationId == notificationId && nr.UserId == userId,
                cancellationToken);
    }

    /// Saves pending Notification Recipient changes asynchronously.
    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }

}
