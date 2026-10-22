using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NotifyX.Api.Entities;

public sealed class IdempotencyKeyConfig
    : IEntityTypeConfiguration<IdempotencyKey>
{
    public void Configure(
        EntityTypeBuilder<IdempotencyKey> modelBuilder)
    {
        modelBuilder.ToTable("IdempotencyKeys");

        modelBuilder.HasKey(x => x.Id);

        modelBuilder.Property(x => x.UserId)
            .IsRequired();

        modelBuilder.Property(x => x.Key)
            .HasMaxLength(200)
            .IsRequired();

        modelBuilder.Property(x => x.RequestHash)
            .HasMaxLength(64)
            .IsRequired();

        modelBuilder.Property(x => x.Status)
            .HasConversion<int>()
            .IsRequired();

        modelBuilder.Property(x => x.CreatedAtUtc)
            .IsRequired();

        // Composite Unique Index
        modelBuilder.HasIndex(x => new
        {
            x.UserId,
            x.Key
        })
        .IsUnique();

        // Optional relationship with Notification
        modelBuilder.HasOne(x => x.Notification)
            .WithMany(x => x.IdempotencyKeys)
            .HasForeignKey(x => x.NotificationId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}