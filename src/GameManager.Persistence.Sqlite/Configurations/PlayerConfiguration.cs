using GameManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameManager.Persistence.Sqlite.Configurations;

public class PlayerConfiguration : IEntityTypeConfiguration<Player>
{
    public void Configure(EntityTypeBuilder<Player> builder)
    {
        builder.HasKey(t => t.Id);

        builder.HasIndex(t => t.GameId);

        builder.Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(20);

        builder.HasOne<Game>()
            .WithMany()
            .HasForeignKey(t => t.GameId);
    }
}