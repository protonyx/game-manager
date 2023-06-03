using GameManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameManager.Persistence.Sqlite;

public class GameContext : DbContext
{
    public DbSet<Game> Games { get; set; }
    
    public DbSet<Player> Players { get; set; }
    
    public DbSet<Tracker> Trackers { get; set; }
    
    public DbSet<TrackerValue> TrackerValues { get; set; }
    
    public DbSet<TrackerHistory> TrackerHistories { get; set; }

    public DbSet<Turn> Turns { get; set; }

    public GameContext(DbContextOptions<GameContext> options)
        : base(options)
    {
        
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
        
        base.OnModelCreating(modelBuilder);
    }
}