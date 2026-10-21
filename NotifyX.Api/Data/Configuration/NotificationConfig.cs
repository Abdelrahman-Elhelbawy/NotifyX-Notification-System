using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NotifyX.Api.Entities;

namespace NotifyX.Api.Data.Configuration;

public class NotificationConfig : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> modelBuilder)
    {
        modelBuilder.ToTable("Notifications");

        modelBuilder.HasKey(n => n.Id);

        modelBuilder.Property(n => n.Title)
            .IsRequired()
            .HasMaxLength(200);

        modelBuilder.Property(n => n.Body)
            .IsRequired()
            .HasMaxLength(1000);

        modelBuilder.Property(n => n.Type)
            .IsRequired();

        modelBuilder.Property(n => n.Priority)
            .IsRequired()
            .HasDefaultValue(0);

        modelBuilder.Property(n => n.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        modelBuilder.Property(n => n.ScheduledAt)
            .IsRequired(false);

        modelBuilder.Property(n => n.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        modelBuilder.Property(n => n.Metadata)
            .IsRequired(false)
            .HasColumnType("nvarchar(max)");


        /*
            1- Add check constraint for Type column to ensure it only contains valid values (1, 2, 3, 4).
            2- Add check constraint for IsRead and ReadAtUtc columns to ensure that if IsRead is true, ReadAtUtc must not be null, and if IsRead is false, ReadAtUtc must be null.
        */
        modelBuilder.ToTable("Notifications", table =>
        {
            table.HasCheckConstraint(
                "CK_Notifications_Type",
                "[Type] IN (1, 2, 3, 4)");
        });
    }
}
