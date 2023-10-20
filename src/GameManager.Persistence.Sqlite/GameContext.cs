using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Logging;

namespace GameManager.Persistence.Sqlite;

public class GameContext : DbContext
{
    private readonly string _connectionString;

    private readonly bool _enableCommandLogging;

    public GameContext(string connectionString, bool enableCommandLogging)
    {
        _connectionString = connectionString;
        _enableCommandLogging = enableCommandLogging;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite(_connectionString);

        if (_enableCommandLogging)
        {
            optionsBuilder.UseLoggerFactory(BuildLoggerFactory());
            optionsBuilder.EnableSensitiveDataLogging();
        }
    }

    private ILoggerFactory BuildLoggerFactory()
    {
        return LoggerFactory.Create(logger =>
        {
            logger.AddFilter((category, level) =>
                category == DbLoggerCategory.Database.Command.Name && level == LogLevel.Information);
            logger.AddConsole();
        });
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