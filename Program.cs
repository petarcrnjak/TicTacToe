using Microsoft.EntityFrameworkCore;
using TicTacToe.Authorization;
using TicTacToe.Database;
using TicTacToe.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSwaggerWithJwt();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseInMemoryDatabase("AuthDb");
});

// Identity via extension
builder.Services.AddAppIdentity();

// JWT via extension
builder.Services.AddJwtAuthentication(builder.Configuration);

builder.Services.AddAuthorization();

builder.Services.AddScoped<ITokenService, JwtTokenService>();
builder.Services.AddScoped<TicTacToe.Services.IAuthService, TicTacToe.Services.AuthService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/health", () => Results.Ok(new { status = "Healthy", utcNow = DateTime.UtcNow }));

app.UseHttpsRedirection();

// Global exception handling (middleware)
app.UseGlobalExceptionHandling();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
