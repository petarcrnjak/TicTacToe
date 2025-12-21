using Dapper;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.Common;
using TicTacToe.Contracts.Dtos;
using TicTacToe.Contracts.Requests;
using TicTacToe.Database.Models;

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

        var playerId = request.UserId.ToString();
        var userExists = await connection.ExecuteScalarAsync<long>(
            new CommandDefinition("SELECT COUNT(1) FROM AspNetUsers WHERE Id = @Id",
                                 new { Id = playerId }, cancellationToken: cancellation));

        if (userExists == 0)
        {
            throw new InvalidOperationException($"Cannot create game: user '{playerId}' does not exist.");
        }

        var newId = await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(insertSql, new
            {
                PlayerX = playerId,
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
            SELECT Id, PlayerX, PlayerO, Winner, CreatedAt, Status
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

    public async Task<Games?> GetGameBoardById(int gameId, CancellationToken cancellation)
    {
        await using DbConnection connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellation);

     
        return await connection.QuerySingleOrDefaultAsync<Games>(@"
               SELECT Board
               FROM Games
               WHERE Id=@GameId",
                new { GameId = gameId });
    }
}
