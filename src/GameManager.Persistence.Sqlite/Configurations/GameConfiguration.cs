using GameManager.Domain.Entities;
using GameManager.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameManager.Persistence.Sqlite.Configurations;

public class GameConfiguration : IEntityTypeConfiguration<Game>
{
    public void Configure(EntityTypeBuilder<Game> builder)
    {
        builder.HasKey(t => t.Id);
        
        builder.Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(100);
        
        builder.Property(t => t.EntryCode)
            .HasConversion(
                v => v.Value,
                v => EntryCode.Of(v))
            .IsRequired()
            .HasMaxLength(10);
        
        builder.OwnsOne(t => t.Options, build =>
        {
            build.ToTable("GameOptions");
        });

        builder.OwnsOne(t => t.CurrentTurn, build =>
        {
            build.Property(t => t.PlayerId).HasColumnName("CurrentTurnPlayerId");
            build.Property(t => t.StartTime).HasColumnName("CurrentTurnStartTime");
        });

        builder.HasIndex(t => t.EntryCode)
            .IsUnique();
    }
}