using Dapper;
using System.Data.Common;
using TicTacToe.Contracts.Requests;
using TicTacToe.Database.Models;
using TicTacToe.Enums;

namespace TicTacToe.Database.Repository;

public sealed class GamesRepository : IGamesRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public GamesRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<int?> CreateGameAsync(CreateGameRequest request, CancellationToken cancellation = default)
    {
        const string insertSql = @"
            INSERT INTO Games (PlayerX, CreatedAt, Status)
            VALUES (@PlayerX, @CreatedAt, @Status);
            SELECT last_insert_rowid();";

        await using DbConnection connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellation);

        var newId = await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(insertSql, new
            {
                PlayerX = request.UserId.ToString(),
                CreatedAt = request.CreatedUtc,
                Status = (int)request.Status
            }, cancellationToken: cancellation));

        return newId;
    }

    public async Task<IReadOnlyCollection<Games>> GetGamesAsync(int page, int pageSize, CancellationToken cancellation = default)
    {
        await using DbConnection connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellation);

        var sql = @"
            SELECT Id, PlayerX, PlayerO, NextTurn, Winner, CreatedAt, Status
            FROM Games
            ORDER BY CreatedAt DESC
            LIMIT @PageSize OFFSET @Offset;";

        var rows = await connection.QueryAsync<Games>(
            new CommandDefinition(sql, new
            {
                PageSize = pageSize,
                Offset = Math.Max(0, (page - 1) * pageSize)
            }, cancellationToken: cancellation));

        return rows.ToList();
    }

    public async Task<Games?> GetGameBoardById(int gameId, CancellationToken cancellation = default)
    {
        await using DbConnection connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellation);


        return await connection.QuerySingleOrDefaultAsync<Games>(@"
               SELECT Board, NextTurn
               FROM Games
               WHERE Id=@GameId",
                new { GameId = gameId });
    }

    public async Task<Games?> GetOpenGameByIdAsync(int gameId, CancellationToken cancellation = default)
    {
        await using DbConnection connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellation);

        return await connection.QuerySingleOrDefaultAsync<Games>(@"
               SELECT Id, PlayerX
               FROM Games
               WHERE Id=@GameId AND Status=@Status",
                new { GameId = gameId, Status = GameStatus.Open });
    }

    public async Task<bool> JoinGameAsync(int gameId, string player1, string userId, CancellationToken cancellation = default)
    {
        const string updateSql = @"
            UPDATE Games
            SET PlayerO = @UserId,
                StartedAt = @StartedAt,
                Status = @NewStatus,
                NextTurn=@Player1
            WHERE Id = @GameId
              AND Status = @ExpectedStatus
              AND (PlayerO IS NULL OR PlayerO = '');";

        await using DbConnection connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellation);

        var affected = await connection.ExecuteAsync(
            new CommandDefinition(updateSql, new
            {
                UserId = userId,
                StartedAt = DateTime.UtcNow,
                Player1 = player1,
                NewStatus = (int)GameStatus.InProgress,
                ExpectedStatus = (int)GameStatus.Open,
                GameId = gameId
            }, cancellationToken: cancellation));

        return affected > 0;
    }

    public async Task<Games?> GetGameByIdAsync(int gameId, CancellationToken cancellation = default)
    {
        await using DbConnection connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellation);

        return await connection.QuerySingleOrDefaultAsync<Games>(@"
               SELECT Id, PlayerX, PlayerO, Board, NextTurn, Winner, CreatedAt, StartedAt, Status
               FROM Games
               WHERE Id = @GameId",
            new { GameId = gameId });
    }

    public async Task<Games?> MakeMoveAsync(MakeMoveRequest request, CancellationToken cancellation = default)
    {
        await using DbConnection connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellation);

        const string updateSql = @"
            UPDATE Games
            SET Board = @Board,
                NextTurn = @NextTurn,
                Winner = @Winner,
                Status = @NewStatus
            WHERE Id = @GameId
              AND NextTurn = @ExpectedNext
              AND Status = @ExpectedStatus;";

        var affected = await connection.ExecuteAsync(new CommandDefinition(updateSql, new
        {
            Board = request.BoardString,
            NextTurn = request.PlayerOpponent,
            Winner = request.Winner,
            NewStatus = (int)request.Status,
            GameId = request.GameId,
            ExpectedNext = request.PlayerMaker,
            ExpectedStatus = (int)GameStatus.InProgress
        }, cancellationToken: cancellation));

        if (affected == 0)
            return null;

        // return updated game
        return await GetGameByIdAsync(request.GameId, cancellation);
    }

    public async Task<string?> GetBoardAsync(int gameId, string currentUserId, CancellationToken cancellation = default)
    {
        await using DbConnection connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellation);

        return await connection.QuerySingleOrDefaultAsync<string>(@"
               SELECT Board
               FROM Games
               WHERE Id=@GameId AND Status=@Status",
                new { GameId = gameId, Status = GameStatus.InProgress });
    }
}
