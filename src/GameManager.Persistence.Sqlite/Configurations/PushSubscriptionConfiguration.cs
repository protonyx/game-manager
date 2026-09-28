using GameManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameManager.Persistence.Sqlite.Configurations;

public class PushSubscriptionConfiguration : IEntityTypeConfiguration<PushSubscription>
{
    public void Configure(EntityTypeBuilder<PushSubscription> builder)
    {
        builder.ToTable("PushSubscriptions");
        builder.HasKey(subscription => subscription.Id);
        builder.Property(subscription => subscription.Endpoint).IsRequired();
        builder.Property(subscription => subscription.P256dh).IsRequired();
        builder.Property(subscription => subscription.Auth).IsRequired();
        builder.HasIndex(subscription => subscription.Endpoint).IsUnique();
        builder.HasIndex(subscription => subscription.PlayerId);
        builder.HasIndex(subscription => subscription.GameId);
        builder.HasOne<Player>().WithMany().HasForeignKey(subscription => subscription.PlayerId);
    }
}
