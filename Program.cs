using Microsoft.EntityFrameworkCore;
using TicTacToe.Authorization;
using TicTacToe.Database;
using TicTacToe.Database.Repository;
using TicTacToe.Extensions;
using TicTacToe.GameEngine;
using TicTacToe.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSwaggerWithJwt();

// SQLite DbContext
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
    "Data Source=tictactoe.db;Cache=Shared;Pooling=True;Mode=ReadWriteCreate;Journal Mode=WAL;BusyTimeout=5000";
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlite(connectionString);
});

// Database initializer
builder.Services.AddScoped<IDbInitializer, DbInitializer>();

// Identity via extension
builder.Services.AddAppIdentity();

// JWT via extension
builder.Services.AddJwtAuthentication(builder.Configuration);

builder.Services.AddAuthorization();

builder.Services.AddScoped<ITokenService, JwtTokenService>()
    .AddScoped<IAuthService, AuthService>()
    .AddScoped<IGamesService, GamesService>()
    .AddScoped<IGameEngine, GameEngine>();

builder.Services.AddSingleton<IDbConnectionFactory>(_ => new SqliteConnectionFactory(connectionString));

builder.Services.AddScoped<IGamesRepository, GamesRepository>()
.AddScoped<IUserContext, UserContext>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
    await dbInitializer.InitializeAsync();
}

app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/health", () => Results.Ok(new { status = "Healthy", utcNow = DateTime.UtcNow }));

app.UseHttpsRedirection();

app.UseGlobalExceptionHandling();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
