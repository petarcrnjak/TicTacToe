using TicTacToe.Contracts;
using TicTacToe.Contracts.Dtos;
using TicTacToe.Contracts.Requests;

namespace TicTacToe.Services
{
    public interface IGameService
    {
        Task<int?> StartGameAsync(CancellationToken cancellation = default);
        Task<List<GameViewDto>> GetGamesAsync(int page, int pageSize, CancellationToken cancellation = default);
        Task<GameFieldDto> GetGameBoardByIdAsync(int gameId, CancellationToken cancellation = default);
        Task<GameStatusDto> JoinGameAsync(int gameId, CancellationToken cancellation = default);
        Task<GameStatusDto> PlayGameAsync(int gameId, GameMove move, CancellationToken cancellation = default);
        Task<List<GameFilterDto>> GetGamesFilterAsync(GamesFilter filter, int page, int pageSize, CancellationToken cancellation = default);
    }
}
