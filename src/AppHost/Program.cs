using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache")
    .WithRedisInsight();

var api = builder.AddProject<GameManager_Server>("api")
    .WithReference(cache);

builder.Build().Run();