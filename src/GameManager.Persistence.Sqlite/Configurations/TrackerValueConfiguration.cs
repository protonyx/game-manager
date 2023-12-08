using GameManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameManager.Persistence.Sqlite.Configurations;

public class TrackerValueConfiguration : IEntityTypeConfiguration<TrackerValue>
{
    public void Configure(EntityTypeBuilder<TrackerValue> builder)
    {
        builder.ToTable("TrackerValues");
        
        builder.HasKey(t => t.Id);

        builder.HasOne<Player>()
            .WithMany(t => t.TrackerValues)
            .HasForeignKey(t => t.PlayerId);

        builder.HasOne<Tracker>()
            .WithMany()
            .HasForeignKey(t => t.TrackerId);
    }
}