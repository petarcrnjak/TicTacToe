using TicTacToe.Contracts;
using TicTacToe.Contracts.Dtos;

namespace TicTacToe.Services
{
    public interface IGamesService
    {
        Task<int?> StartGameAsync(CancellationToken cancellation = default);
        Task<List<GameDto>> GetGamesAsync(int page, int pageSize, CancellationToken cancellation = default);
        Task<GameFieldDto> GetGameBoardByIdAsync(int gameId, CancellationToken cancellation = default);
        Task<GameStatusDto> JoinGameAsync(int gameId, CancellationToken cancellation = default);
        Task<GameStatusDto> PlayGameAsync(int gameId, GameMove move, CancellationToken cancellation = default);
    }
}
