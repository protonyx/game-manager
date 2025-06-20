using GameManager.Domain.Entities;
using GameManager.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameManager.Persistence.Sqlite.Configurations;

public class TrackerConfiguration : IEntityTypeConfiguration<Tracker>
{
    public void Configure(EntityTypeBuilder<Tracker> builder)
    {
        builder.ToTable("Trackers");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Name)
            .HasConversion(
                v => v.Value,
                v => TrackerName.From(v).Value)
            .IsRequired()
            .HasMaxLength(TrackerName.MaximumLength);

        builder.HasOne<Game>()
            .WithMany(t => t.Trackers)
            .HasForeignKey(t => t.GameId);
    }
}