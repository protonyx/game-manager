﻿using GameManager.Domain.Entities;
using GameManager.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameManager.Persistence.Sqlite.Configurations;

public class PlayerConfiguration : IEntityTypeConfiguration<Player>
{
    public void Configure(EntityTypeBuilder<Player> builder)
    {
        builder.ToTable("Players");
        
        builder.HasKey(t => t.Id);

        builder.HasIndex(t => t.GameId);

        builder.Property(t => t.Name)
            .HasConversion(
                v => v.Value,
                v => PlayerName.From(v).Value)
            .IsRequired()
            .HasMaxLength(20);
        builder.Property(t => t.Active)
            .IsRequired();
        builder.Property(t => t.Order);
        builder.Property(t => t.IsAdmin);
        builder.Property(t => t.JoinedDate);
        builder.Property(t => t.LastHeartbeat);

        builder.HasOne<Game>()
            .WithMany()
            .HasForeignKey(t => t.GameId);

        builder.Navigation(t => t.TrackerValues)
            .AutoInclude();
    }
}