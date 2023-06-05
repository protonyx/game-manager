using GameManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameManager.Persistence.Sqlite.Configurations;

public class TurnConfiguration : IEntityTypeConfiguration<Turn>
{
    public void Configure(EntityTypeBuilder<Turn> builder)
    {
        builder.HasKey(t => t.Id);

        builder.HasOne<Player>()
            .WithMany(t => t.Turns)
            .HasForeignKey(t => t.PlayerId);
    }
}