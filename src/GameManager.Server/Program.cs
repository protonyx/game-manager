using System.Diagnostics;
using System.Reflection;
using System.Text.Json.Serialization;
using FastEndpoints;
using FastEndpoints.Swagger;
using GameManager.Application;
using GameManager.Application.Authorization;
using GameManager.Application.Contracts;
using GameManager.Application.Contracts.Persistence;
using GameManager.Application.Profiles;
using GameManager.Persistence.Sqlite;
using GameManager.Server;
using GameManager.Server.Authentication;
using GameManager.Server.Authorization;
using GameManager.Server.DataLoaders;
using GameManager.Server.HostedServices;
using GameManager.Server.Services;
using GameManager.Server.Types;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Converters;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using StackExchange.Redis;

var assm = Assembly.GetEntryAssembly();
string version;

try
{
    var versionInfo = FileVersionInfo.GetVersionInfo(assm.Location);
    version = versionInfo.ProductVersion;
}
catch (Exception)
{
    version = assm.GetName().Version.ToString();
}

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults("game-manager", version);

// Add services to the container.


builder.Services.AddFastEndpoints()
    .SwaggerDocument(o =>
    {
        o.MaxEndpointVersion = 1;
        o.DocumentSettings = s =>
        {
            s.Title = "Game Manager";
            s.Version = "v1";
        };
    });
builder.Services.Configure<JsonOptions>(opt =>
{
    opt.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});
builder.Services.AddEndpointsApiExplorer();

var signalr = builder.Services.AddSignalR()
    .AddNewtonsoftJsonProtocol(opt =>
    {
        opt.PayloadSerializerSettings.Converters.Add(new StringEnumConverter());
    });
builder.Services.AddSingleton<IUserIdProvider, PlayerUserIdProvider>();

builder.Services.AddSingleton<IGameClientNotificationService, GameHubClientNotificationService>();

builder.Services.AddCors(opt =>
{
    opt.AddDefaultPolicy(policy =>
    {
        if (builder.Environment.IsDevelopment())
        {
            policy.WithOrigins("http://localhost:4200")
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
        }
        else
        {
            policy.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
        }
    });
});

var tokenService = new TokenService(builder.Configuration);

builder.Services.AddAuthentication(opt =>
    {
        opt.DefaultScheme = "SchemeSelector";
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = tokenService.GetSigningKey()
        };
        options.EventsType = typeof(CustomJwtBearerEvents);
    })
    .AddBasic(options =>
    {
        builder.Configuration.Bind("Admin", options);
    })
    .AddPolicyScheme("SchemeSelector", "SchemeSelector", options =>
    {
        options.ForwardDefaultSelector = context =>
        {
            string authorization = context.Request.Headers.Authorization;
            if (string.IsNullOrEmpty(authorization) || authorization.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                return JwtBearerDefaults.AuthenticationScheme;
            }
            else
            {
                return BasicAuthenticationHandler.BasicAuthenticationSchemeName;
            }
        };
    });
builder.Services.AddAuthorization(opt =>
{
    opt.AddPolicy(AuthorizationPolicyNames.Admin, policy =>
    {
        policy.AuthenticationSchemes.Add("Basic");
        policy.RequireAuthenticatedUser();
        policy.RequireRole(GameManagerRoles.Admin);
    });
    opt.AddPolicy(AuthorizationPolicyNames.ViewGame, policy =>
    {
        policy.AddRequirements(new GameAuthorizationRequirement(modify: false));
    });
    opt.AddPolicy(AuthorizationPolicyNames.ModifyGame, policy =>
    {
        policy.AddRequirements(new GameAuthorizationRequirement(modify: true));
    });
});
builder.Services.AddScoped<IAuthorizationHandler, GameAuthorizationHandler>();
builder.Services.AddScoped<IAuthorizationHandler, GameResourceAuthorizationHandler>();
builder.Services.AddScoped<IAuthorizationHandler, PlayerAuthorizationHandler>();
builder.Services.AddSingleton<IAuthorizationMiddlewareResultHandler, ProblemAuthorizationMiddlewareResultHandler>();

builder.Services.AddGraphQLServer()
    .AddAuthorization()
    .AddFiltering()
    .AddQueryType<Query>()
    .AddType<GameType>()
    .AddType<PlayerTrackerValueType>()
    //.RegisterService<IPlayerRepository>(ServiceKind.Synchronized)
    .AddDataLoader<PlayerByIdDataLoader>()
    .AddDataLoader<PlayersByGameIdDataLoader>()
    .AddDataLoader<GameByIdDataLoader>()
    .AddDataLoader<TrackerByIdDataLoader>()
    .AddDataLoader<TurnsByGameIdDataLoader>();

builder.Services.AddScoped<CustomJwtBearerEvents>();
builder.Services.AddSingleton<ITokenService>(tokenService);

// Caching
builder.Services.AddMemoryCache();

var redisConnectionString = builder.Configuration.GetConnectionString("cache");

if (string.IsNullOrWhiteSpace(redisConnectionString))
{
    builder.Services.AddDistributedMemoryCache();
}
else
{
    builder.AddRedisDistributedCache("cache");

// var redisConnectionMultiplexer = ConnectionMultiplexer.Connect(redisConnectionString);
// builder.Services.AddSingleton<IConnectionMultiplexer>(redisConnectionMultiplexer);
// builder.Services.AddStackExchangeRedisCache(opt =>
// {
//     opt.ConnectionMultiplexerFactory = () => Task.FromResult<IConnectionMultiplexer>(redisConnectionMultiplexer);
// });
    signalr.AddStackExchangeRedis(redisConnectionString); // TODO: Aspire?
}

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUserContext, HttpContextUserContext>();

builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddMaps(typeof(DtoProfile), typeof(GraphQlProfile));
});

builder.Services.AddApplicationServices();
builder.Services.AddSqlitePersistenceServices();

builder.Services.AddHostedService<GamePruningService>();


var app = builder.Build();

// Run migrations
if (!EF.IsDesignTime)
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<GameContext>();
    db.Database.Migrate();
}

// Configure the HTTP request pipeline.
app.UseStaticFiles();
app.UseRouting();

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.UseFastEndpoints(c =>
{
    c.Endpoints.RoutePrefix = "api";
    c.Endpoints.ShortNames = true;
    c.Versioning.Prefix = "v";
    c.Versioning.DefaultVersion = 1;
    c.Versioning.PrependToRoute = true;
    c.Errors.UseProblemDetails(x =>
    {
        x.TitleTransformer = pd => pd.Status switch
        {
            StatusCodes.Status403Forbidden => "Forbidden",
            _ => "One or more validation errors occurred."
        };
    });
    c.Errors.ResponseBuilder = (failures, ctx, statusCode) =>
        new ProblemDetails(failures, ctx.Request.Path, Activity.Current.Id, statusCode);
});
app.UseSwaggerGen();

app.MapHub<GameHub>("/hubs/game");
app.MapGraphQL();
app.MapFallbackToFile("index.html");
app.MapGet("/version", async ctx =>
{
    await ctx.Response.WriteAsJsonAsync(new
    {
        Version = version
    });
});
app.MapDefaultEndpoints();

app.Run();

namespace GameManager.Server
{
    public partial class Program
    {
    }
}