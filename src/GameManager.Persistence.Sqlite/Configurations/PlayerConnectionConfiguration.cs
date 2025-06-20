using GameManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameManager.Persistence.Sqlite.Configurations;

public class PlayerConnectionConfiguration : IEntityTypeConfiguration<PlayerConnection>
{
    public void Configure(EntityTypeBuilder<PlayerConnection> builder)
    {
        builder.ToTable("PlayerConnections");

        builder.HasKey(t => new { t.PlayerId, t.ConnectionId });

        builder.Property(t => t.ConnectionId)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(t => t.ConnectedDate);
        builder.Property(t => t.LastHeartbeat);

        builder.HasOne<Player>()
            .WithMany(t => t.Connections)
            .HasForeignKey(t => t.PlayerId);
    }
}