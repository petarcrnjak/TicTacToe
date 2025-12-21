using TicTacToe.Contracts.Dtos;

namespace TicTacToe.Services
{
    public interface IGamesService
    {
        Task<int?> CreateGameAsync(CancellationToken cancellation = default);
        Task<List<GameDto>> GetGamesAsync(int page, int pageSize, CancellationToken cancellation = default);
        Task<GameFieldDto> GetGameBoardByIdAsync(int gameId, CancellationToken cancellation = default);
    }
}
