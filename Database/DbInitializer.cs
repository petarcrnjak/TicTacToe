using Microsoft.EntityFrameworkCore;

namespace TicTacToe.Database;

public sealed class DbInitializer : IDbInitializer
{
    private readonly AppDbContext _context;

    public DbInitializer(AppDbContext context)
    {
        _context = context;
    }

    public async Task InitializeAsync()
    {
        await _context.Database.OpenConnectionAsync();

        await _context.Database.EnsureCreatedAsync();

        var createGamesSql = @"
            CREATE TABLE IF NOT EXISTS Games (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            PlayerX TEXT,
            PlayerO TEXT,
            Board TEXT NOT NULL DEFAULT '0,0,0,0,0,0,0,0,0',
            NextTurn TEXT,
            Winner TEXT,
            CreatedAt TEXT NOT NULL,
            StartedAt TEXT,
            Status INTEGER NOT NULL,
            FOREIGN KEY (PlayerX) REFERENCES AspNetUsers(Id) ON DELETE RESTRICT,
            FOREIGN KEY (PlayerO) REFERENCES AspNetUsers(Id) ON DELETE RESTRICT
            );";
        await _context.Database.ExecuteSqlRawAsync(createGamesSql);
    }
}