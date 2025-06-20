using GameManager.Domain.Entities;
using GameManager.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameManager.Persistence.Sqlite.Configurations;

public class GameConfiguration : IEntityTypeConfiguration<Game>
{
    public void Configure(EntityTypeBuilder<Game> builder)
    {
        builder.ToTable("Games");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Name)
            .HasConversion(
                v => v.Value,
                v => GameName.From(v).Value)
            .IsRequired()
            .HasMaxLength(GameName.MaximumLength);

        builder.Property(t => t.EntryCode)
            .HasConversion(
                v => v.Value,
                v => EntryCode.From(v).Value)
            .HasMaxLength(EntryCode.MaximumLength);

        builder.Property(t => t.ETag)
            .HasConversion(
                v => v.Value.ToString(),
                v => ETag.From(v))
            .IsConcurrencyToken();

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

        builder.Navigation(t => t.Trackers)
            .AutoInclude();
    }
}