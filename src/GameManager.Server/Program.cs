using System.Text;
using GameManager.Server;
using GameManager.Server.Authentication;
using GameManager.Server.Data;
using GameManager.Server.Profiles;
using GameManager.Server.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddNewtonsoftJson();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSignalR()
    .AddJsonProtocol();

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

builder.Services.AddAuthentication(opt =>
    {
        opt.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        var key = builder.Configuration["Jwt:Key"];
        byte[] keyBytes;
        if (string.IsNullOrWhiteSpace(key))
        {
            // Generate a random key
            keyBytes = new byte[16];
            Random.Shared.NextBytes(keyBytes);
            TokenService.DefaultKey = keyBytes;
        }
        else
        {
            keyBytes = Convert.FromBase64String(key);
        }
        
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(keyBytes)
        };
        options.EventsType = typeof(CustomJwtBearerEvents);
    });

builder.Services.AddDbContext<GameContext>((sp, opt) =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    opt.UseSqlite(config.GetConnectionString("Database"));
});
builder.Services.AddScoped<CustomJwtBearerEvents>();
builder.Services.AddScoped<GameRepository>();
builder.Services.AddScoped<PlayerRepository>();

builder.Services.AddSingleton<TokenService>();
builder.Services.AddScoped<GameStateService>();

builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<DtoProfile>();
});
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
});

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

app.MapControllers();
app.MapHub<GameHub>("/hubs/game");
app.MapFallbackToFile("index.html");
app.MapSwagger();

app.Run();

public partial class Program
{
}