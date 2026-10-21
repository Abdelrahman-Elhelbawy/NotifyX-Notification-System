using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NotifyX.Api.Entities;

namespace NotifyX.Api.Data.Configuration;

public class NotificationRecipientConfig : IEntityTypeConfiguration<NotificationRecipient>
{
    public void Configure(EntityTypeBuilder<NotificationRecipient> modelBuilder)
    {
        modelBuilder.ToTable("NotificationRecipients");

        modelBuilder.HasKey(x => new
        {
            x.NotificationId,
            x.UserId
        });

        modelBuilder.Property(n => n.IsRead)
            .IsRequired()
            .HasDefaultValue(false);

        modelBuilder.HasOne(x => x.Notification)
            .WithMany(x => x.Recipients)
            .HasForeignKey(x => x.NotificationId)
            .OnDelete(DeleteBehavior.Cascade);

        /*
            1- Add check constraint for IsRead and ReadAtUtc columns to ensure that if IsRead is true, ReadAtUtc must not be null, and if IsRead is false, ReadAtUtc must be null.
        */
        modelBuilder.ToTable("NotificationRecipients", table =>
        {
            table.HasCheckConstraint(
                "CK_Notifications_ReadState",
                "([IsRead] = 0 AND [ReadAtUtc] IS NULL) OR " +
                "([IsRead] = 1 AND [ReadAtUtc] IS NOT NULL)");
        });
    }
}
