using Microsoft.EntityFrameworkCore;

namespace TicTacToe.Database;

public interface IDbInitializer
{
    Task InitializeAsync();
}

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

        var createIndexesSql = @"
            CREATE INDEX IF NOT EXISTS IX_Games_CreatedAt_DESC ON Games (CreatedAt DESC);
            CREATE INDEX IF NOT EXISTS IX_Games_StartedAt ON Games (StartedAt);
            CREATE INDEX IF NOT EXISTS IX_Games_Status ON Games (Status);
            CREATE INDEX IF NOT EXISTS IX_Games_PlayerX ON Games (PlayerX);
            CREATE INDEX IF NOT EXISTS IX_Games_PlayerO ON Games (PlayerO);
            CREATE INDEX IF NOT EXISTS IX_Games_Winner ON Games (Winner);
            CREATE INDEX IF NOT EXISTS IX_Games_NextTurn ON Games (NextTurn);
        ";
        await _context.Database.ExecuteSqlRawAsync(createIndexesSql);
    }
}