using GameManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameManager.Persistence.Sqlite.Configurations;

public class GameOptionsConfiguration : IEntityTypeConfiguration<GameOptions>
{
    public void Configure(EntityTypeBuilder<GameOptions> builder)
    {
        builder.HasKey(t => t.GameId);
    }
}