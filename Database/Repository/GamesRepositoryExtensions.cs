using TicTacToe.Database.Exceptions;
using TicTacToe.Database.Models;

namespace TicTacToe.Database.Repository;

public static class GamesRepositoryExtensions
{
    public static async Task<Games> GetGameByIdOrThrowAsync(this IGamesRepository repo, int gameId, CancellationToken cancellation, string? errorMessage = null)
    {
        var game = await repo.GetGameByIdAsync(gameId, cancellation);
        if (game == null)
            throw new NotFoundException(string.Format(errorMessage ?? "Game with id {0} was not found", gameId));
        return game;
    }

    public static async Task<Games> GetOpenGameByIdOrThrowAsync(this IGamesRepository repo, int gameId, CancellationToken cancellation, string? errorMessage = null)
    {
        var game = await repo.GetOpenGameByIdAsync(gameId, cancellation);
        if (game == null)
            throw new NotFoundException(string.Format(errorMessage ?? "Game with id {0} was not found", gameId));
        return game;
    }

    public static async Task<Games> GetGameBoardByIdOrThrowAsync(this IGamesRepository repo, int gameId, CancellationToken cancellation, string? errorMessage = null)
    {
        var game = await repo.GetGameBoardById(gameId, cancellation);
        if (game == null)
            throw new NotFoundException(string.Format(errorMessage ?? "Game with id {0} was not found", gameId));
        return game;
    }
}