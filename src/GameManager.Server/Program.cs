using System.Diagnostics;
using System.Reflection;
using System.Text.Json.Serialization;
using FastEndpoints;
using FastEndpoints.Swagger;
using GameManager.Application;
using GameManager.Application.Contracts;
using GameManager.Persistence.Sqlite;
using GameManager.Server;
using GameManager.Server.Authentication;
using GameManager.Server.Authorization;
using GameManager.Server.HostedServices;
using GameManager.Server.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Converters;
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

// Add services to the container.
var redisConnectionString = builder.Configuration.GetConnectionString("Redis");

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
        opt.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
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
    });
builder.Services.AddAuthorization();
builder.Services.AddSingleton<IAuthorizationHandler, GameAuthorizationHandler>();
builder.Services.AddSingleton<IAuthorizationHandler, PlayerAuthorizationHandler>();
builder.Services.AddSingleton<IAuthorizationMiddlewareResultHandler, ProblemAuthorizationMiddlewareResultHandler>();

builder.Services.AddScoped<CustomJwtBearerEvents>();
builder.Services.AddSingleton<ITokenService>(tokenService);

// Caching
builder.Services.AddMemoryCache();

if (string.IsNullOrWhiteSpace(redisConnectionString))
{
    builder.Services.AddDistributedMemoryCache();
}
else
{
    var redisConnectionMultiplexer = ConnectionMultiplexer.Connect(redisConnectionString);
    builder.Services.AddSingleton<IConnectionMultiplexer>(redisConnectionMultiplexer);
    builder.Services.AddStackExchangeRedisCache(opt =>
    {
        opt.ConnectionMultiplexerFactory = () => Task.FromResult<IConnectionMultiplexer>(redisConnectionMultiplexer);
    });
    signalr.AddStackExchangeRedis(redisConnectionString);
}

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUserContext, HttpContextUserContext>();

builder.Services.AddApplicationServices();
builder.Services.AddSqlitePersistenceServices();

builder.Services.AddHostedService<GamePruningService>();

// TODO: Switch to using OTEL_EXPORTER_OTLP_ENDPOINT
var otlpEndpoint = builder.Configuration.GetValue<string>("Otlp:Endpoint");

var resources = ResourceBuilder.CreateDefault()
    .AddService("game-manager",
        serviceInstanceId: Environment.MachineName,
        serviceVersion: version);

var otelBuilder = builder.Services.AddOpenTelemetry()
    .WithMetrics(mb =>
    {
        mb.SetResourceBuilder(resources);
        mb.AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation();
        mb.AddPrometheusExporter(prom =>
        {
            prom.ScrapeEndpointPath = "/metrics";
        });
    });

if (!string.IsNullOrWhiteSpace(otlpEndpoint))
{
    otelBuilder
        .WithTracing(tb =>
        {
            tb.SetResourceBuilder(resources);
            tb.AddAspNetCoreInstrumentation(opt =>
                {
                    opt.Filter = hc => !hc.Request.Method.Equals("OPTIONS")
                                       && !hc.Request.Path.StartsWithSegments("/health")
                                       && !hc.Request.Path.StartsWithSegments("/metrics");
                })
                .AddHttpClientInstrumentation()
                .AddEntityFrameworkCoreInstrumentation();

            if (!string.IsNullOrWhiteSpace(redisConnectionString))
            {
                tb.AddRedisInstrumentation();
            }

            tb.AddOtlpExporter(otlp =>
            {
                otlp.Endpoint = new Uri(otlpEndpoint);
                otlp.Protocol = OtlpExportProtocol.Grpc;
            });
        });

    builder.Logging.AddOpenTelemetry(opt =>
    {
        opt.IncludeFormattedMessage = false;
        opt.SetResourceBuilder(resources);
        opt.AddOtlpExporter(otlp =>
        {
            otlp.Endpoint = new Uri(otlpEndpoint);
            otlp.Protocol = OtlpExportProtocol.Grpc;
        });
    });
}

var app = builder.Build();

// Run migrations
using (var scope = app.Services.CreateScope())
{
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
    c.Errors.UseProblemDetails();
    c.Errors.ResponseBuilder = (failures, ctx, statusCode) =>
        new ProblemDetails(failures, ctx.Request.Path, Activity.Current.Id, statusCode);
    ProblemDetails.TitleTransformer = pd =>
    {
        if (pd.Status == StatusCodes.Status403Forbidden)
        {
            return "Forbidden";
        }
        else
        {
            return "One or more validation errors occurred.";
        }
    };
});
app.UseSwaggerGen();

app.UseOpenTelemetryPrometheusScrapingEndpoint();

app.MapHub<GameHub>("/hubs/game");
app.MapFallbackToFile("index.html");
app.MapGet("/version", async ctx =>
{
    await ctx.Response.WriteAsJsonAsync(new
    {
        Version = version
    });
});

app.Run();

namespace GameManager.Server
{
    public partial class Program
    {
    }
}