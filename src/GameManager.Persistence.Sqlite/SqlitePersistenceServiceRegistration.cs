﻿using GameManager.Application.Contracts.Persistence;
using GameManager.Persistence.Sqlite.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GameManager.Persistence.Sqlite;

public static class SqlitePersistenceServiceRegistration
{
    public static IServiceCollection AddSqlitePersistenceServices(this IServiceCollection services)
    {
        services.AddScoped<GameContext>(sp =>
        {
            var config = sp.GetRequiredService<IConfiguration>();
            var cs = config.GetConnectionString("Database");

            return new GameContext(cs, true);
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