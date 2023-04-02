using GameManager.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace GameManager.Server.Data;

public class GameContext : DbContext
{
    public DbSet<Game> Games { get; set; }
    
    public DbSet<Player> Players { get; set; }
    
    public DbSet<Tracker> Trackers { get; set; }
    
    public DbSet<TrackerValue> TrackerValues { get; set; }

    public GameContext(DbContextOptions<GameContext> options)
    : base(options)
    {
        
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Game>(e =>
        {
            e.HasKey(t => t.Id);

            e.HasOne(t => t.Options)
                .WithOne();

            e.HasIndex(t => t.EntryCode)
                .IsUnique();
        });
        modelBuilder.Entity<GameOptions>(e =>
        {
            e.HasKey(t => t.GameId);
        });
        modelBuilder.Entity<Player>(e =>
        {
            e.HasKey(t => t.Id);

            e.HasIndex(t => t.GameId);

            e.HasOne(t => t.Game)
                .WithMany(t => t.Players)
                .HasForeignKey(t => t.GameId);
        });
        modelBuilder.Entity<Tracker>(e =>
        {
            e.HasKey(t => t.Id);

            e.HasOne(t => t.Game)
                .WithMany(t => t.Trackers)
                .HasForeignKey(t => t.GameId);
        });
        modelBuilder.Entity<TrackerValue>(e =>
        {
            e.HasKey(t => t.Id);

            e.HasOne(t => t.Player)
                .WithMany(t => t.TrackerValues)
                .HasForeignKey(t => t.PlayerId);

            e.HasOne(t => t.Tracker)
                .WithMany()
                .HasForeignKey(t => t.TrackerId);
        });
    }
}