using GameManager.Application;
using GameManager.Application.Contracts;
using GameManager.Application.Services;
using GameManager.Persistence.Sqlite;
using GameManager.Server;
using GameManager.Server.Authentication;
using GameManager.Server.Filters;
using GameManager.Server.HostedServices;
using GameManager.Server.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Converters;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(opt =>
    {
        opt.Filters.Add<RequireActivePlayerFilter>();
    })
    .AddNewtonsoftJson(opt =>
    {
        opt.SerializerSettings.Converters.Add(new StringEnumConverter());
    });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSignalR()
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


builder.Services.AddScoped<CustomJwtBearerEvents>();

builder.Services.AddSingleton<ITokenService>(tokenService);
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUserContext, HttpContextUserContext>();

builder.Services.AddApplicationServices();
builder.Services.AddSqlitePersistenceServices();

builder.Services.AddHostedService<GamePruningService>();

var otlpEndpoint = builder.Configuration.GetValue<string>("Otlp:Endpoint");

if (!string.IsNullOrWhiteSpace(otlpEndpoint))
{
    var resources = ResourceBuilder.CreateDefault()
        .AddService("game-manager", serviceInstanceId: Environment.MachineName);
    
    builder.Services.AddOpenTelemetry()
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
            tb.AddOtlpExporter(otlp =>
            {
                otlp.Endpoint = new Uri(otlpEndpoint);
                otlp.Protocol = OtlpExportProtocol.Grpc;
            });
        })
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

app.UseSwaggerUI();
app.UseOpenTelemetryPrometheusScrapingEndpoint();

app.MapControllers();
app.MapHub<GameHub>("/hubs/game");
app.MapFallbackToFile("index.html");
app.MapSwagger();

app.Run();

namespace GameManager.Server
{
    public partial class Program
    {
    }
}