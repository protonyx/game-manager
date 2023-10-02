using GameManager.Application.Contracts.Persistence;
using GameManager.Persistence.Sqlite.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GameManager.Persistence.Sqlite;

public static class SqlitePersistenceServiceRegistration
{
    public static IServiceCollection AddSqlitePersistenceServices(this IServiceCollection services)
    {
        services.AddDbContext<GameContext>((sp, opt) =>
        {
            var config = sp.GetRequiredService<IConfiguration>();
            opt.UseSqlite(config.GetConnectionString("Database"));
        });
        
        // Repositories
        services.AddScoped(typeof(IAsyncRepository<>), typeof(BaseRepository<>));
        services.AddScoped<IGameRepository, GameRepository>();
        services.AddScoped<IPlayerRepository, PlayerRepository>();
        services.AddScoped<ITrackerHistoryRepository, TrackerHistoryRepository>();
        services.AddScoped<ITurnRepository, TurnRepository>();

        return services;
    }
}