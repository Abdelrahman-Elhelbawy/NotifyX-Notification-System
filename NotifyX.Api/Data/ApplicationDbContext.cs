using Microsoft.EntityFrameworkCore;
using NotifyX.Api.Data.Configuration;
using NotifyX.Api.Entities;

namespace NotifyX.Api.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : DbContext(options)
{
    public DbSet<Notification> Notifications { get; set; } = null!;
    public DbSet<NotificationRecipient> NotificationRecipients { get; set; } = null!;

    protected override void OnModelCreating(
    ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        /*
            -- Apply all configurations from the assembly containing the ApplicationDbContext class.
                -- NotificationConfig class is automatically discovered and applied because it implements IEntityTypeConfiguration<Notification>.
        */
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(ApplicationDbContext).Assembly);
    }
}