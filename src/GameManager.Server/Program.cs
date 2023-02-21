using System.Text;
using GameManager.Server;
using GameManager.Server.Data;
using GameManager.Server.Profiles;
using GameManager.Server.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSignalR();

builder.Services.AddCors(opt =>
{
    opt.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

builder.Services.AddAuthentication()
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
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

builder.Services.AddDbContext<GameContext>((sp, opt) =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    // var csb = new SqliteConnectionStringBuilder()
    // {
    //     DataSource = "gm.db",
    // };
    opt.UseSqlite(config.GetConnectionString("Database"));
});
builder.Services.AddScoped<GameRepository>();
builder.Services.AddScoped<PlayerRepository>();

builder.Services.AddSingleton<TokenService>();

builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<DtoProfile>();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.UseCors();

app.MapControllers();
app.MapHub<GameHub>("/hubs/game");

app.Run();

public partial class Program
{
}