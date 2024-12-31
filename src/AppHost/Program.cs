using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache")
    .WithRedisInsight();

var api = builder.AddProject<GameManager_Server>("api")
    .WithReference(cache)
    .WaitFor(cache);

var app = builder.AddNpmApp("angular", "../../web")
    .WithReference(api)
    .WaitFor(api)
    .WithHttpEndpoint(env: "PORT", port: 4200)
    .WithExternalHttpEndpoints()
    .PublishAsDockerFile();

builder.Build().Run();