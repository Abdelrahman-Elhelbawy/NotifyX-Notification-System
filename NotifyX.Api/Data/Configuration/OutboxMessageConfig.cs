using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NotifyX.Api.Entities;

namespace NotifyX.Api.Data.Configuration;

public sealed class OutboxMessageConfig
    : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(
        EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.ToTable("OutboxMessages");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.EventType)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Payload)
            .IsRequired();

        builder.Property(x => x.LastError)
            .HasMaxLength(4000);

        builder.HasIndex(x => new
        {
            x.ProcessedAtUtc,
            x.DeadLetteredAtUtc,
            x.NextAttemptAtUtc
        });
    }
}
