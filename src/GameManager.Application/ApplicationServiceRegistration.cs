using System.Reflection;
using FluentValidation;
using GameManager.Application.Pipelines;
using Microsoft.Extensions.DependencyInjection;

namespace GameManager.Application;

public static class ApplicationServiceRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(typeof(ApplicationServiceRegistration).Assembly);
#if DEBUG
            cfg.AddOpenBehavior(typeof(LoggingPipelineBehavior<,>));
#endif
            cfg.AddOpenBehavior(typeof(ValidationPipelineBehavior<,>));
        });

        return services;
    }
}