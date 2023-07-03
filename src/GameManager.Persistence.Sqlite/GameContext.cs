using GameManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

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
        
        // Treat all DateTime properties as UTC
        var dateTimeConverter = new ValueConverter<DateTime, DateTime>(
            v => v.ToUniversalTime(),
            v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

        var nullableDateTimeConverter = new ValueConverter<DateTime?, DateTime?>(
            v => v.HasValue ? v.Value.ToUniversalTime() : v,
            v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : v);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (entityType.IsKeyless)
            {
                continue;
            }

            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(DateTime))
                {
                    property.SetValueConverter(dateTimeConverter);
                }
                else if (property.ClrType == typeof(DateTime?))
                {
                    property.SetValueConverter(nullableDateTimeConverter);
                }
            }
        }
        
        base.OnModelCreating(modelBuilder);
    }
}