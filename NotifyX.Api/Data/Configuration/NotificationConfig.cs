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

        modelBuilder.Property(n => n.IsRead)
            .IsRequired()
            .HasDefaultValue(false);

        modelBuilder.Property(n => n.Metadata)
            .IsRequired(false)
            .HasColumnType("nvarchar(max)");
        
    }
}
