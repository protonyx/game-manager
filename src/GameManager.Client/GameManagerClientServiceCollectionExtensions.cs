using Microsoft.Extensions.DependencyInjection;

namespace GameManager.Client;

public static class GameManagerClientServiceCollectionExtensions
{
    public static IServiceCollection AddGameManagerClient2(this IServiceCollection services)
    {
        services.AddGameManagerClient();

        return services;
    }
}