using TicTacToe.Contracts.Dtos;
using TicTacToe.Contracts.Requests;
using TicTacToe.Database.Models;

namespace TicTacToe.Database.Repository;

public interface IGamesRepository
{
    Task<int?> CreateGameAsync(CreateGameRequest request, CancellationToken cancellation = default);
    Task<IReadOnlyCollection<Games>> GetGamesAsync(int page, int pageSize, CancellationToken cancellation = default);
    Task<Games?> GetGameBoardById(int gameId, CancellationToken cancellation);
}
