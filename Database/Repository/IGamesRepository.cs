using TicTacToe.Contracts.Requests;
using TicTacToe.Database.Models;

namespace TicTacToe.Database.Repository;

public interface IGamesRepository
{
    Task<int?> CreateGameAsync(CreateGameRequest request, CancellationToken cancellation = default);
    Task<IReadOnlyCollection<Games>> GetGamesAsync(int page, int pageSize, CancellationToken cancellation = default);
    Task<Games?> GetGameBoardById(int gameId, CancellationToken cancellation = default);
    Task<Games?> GetOpenGameByIdAsync(int gameId, CancellationToken cancellation = default);
    Task<bool> JoinGameAsync(int gameId, string player1, string currentUserId, CancellationToken cancellation = default);
    Task<Games?> GetGameByIdAsync(int gameId, CancellationToken cancellation = default);
    Task<Games?> MakeMoveAsync(MakeMoveRequest moveRequest, CancellationToken cancellation = default);
    Task<IReadOnlyCollection<Games>> GetGamesFilteredAsync(GamesFilter filter, int page, int pageSize, CancellationToken cancellation = default);
}
