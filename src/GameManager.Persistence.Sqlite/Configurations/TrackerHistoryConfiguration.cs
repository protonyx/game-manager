using GameManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameManager.Persistence.Sqlite.Configurations;

public class TrackerHistoryConfiguration : IEntityTypeConfiguration<TrackerHistory>
{
    public void Configure(EntityTypeBuilder<TrackerHistory> builder)
    {
        builder.ToTable("TrackerHistories");

        builder.HasKey(t => t.Id);

        builder.HasOne<Player>()
            .WithMany()
            .HasForeignKey(t => t.PlayerId);

        builder.HasOne<Tracker>()
            .WithMany()
            .HasForeignKey(t => t.TrackerId);
    }
}