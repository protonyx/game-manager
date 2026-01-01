using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache")
    .WithRedisInsight(c =>
    {
        c.WithExternalHttpEndpoints();
    })
    .WithLifetime(ContainerLifetime.Persistent);

var api = builder.AddProject<GameManager_Server>("api")
    .WithReference(cache)
    .WaitFor(cache);

var app = builder.AddJavaScriptApp("angular", "../../web", "start")
    .WithReference(api)
    .WaitFor(api)
    .WithHttpEndpoint(env: "PORT", port: 4200)
    .WithExternalHttpEndpoints()
    .PublishAsDockerFile()
    .WithExplicitStart();

builder.Build().Run();