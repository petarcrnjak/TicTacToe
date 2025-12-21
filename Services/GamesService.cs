using TicTacToe.Authorization;
using TicTacToe.Contracts.Dtos;
using TicTacToe.Contracts.Requests;
using TicTacToe.Database.Models;
using TicTacToe.Database.Repository;
using TicTacToe.Exceptions;

namespace TicTacToe.Services;

public class GamesService : IGamesService
{
    private readonly IGamesRepository _gamesRepository;
    private readonly IUserContext _userContext;

    public GamesService(IGamesRepository gamesRepository, IUserContext userContext)
    {
        _gamesRepository = gamesRepository;
        _userContext = userContext;
    }

    public async Task<int?> CreateGameAsync(CancellationToken cancellation = default)
    {
        var newGame = new CreateGameRequest
        {
            UserId = _userContext.UserId,
            CreatedUtc = DateTime.UtcNow,
            Status = Enums.GameStatus.Open
        };

        var id = await _gamesRepository.CreateGameAsync(newGame, cancellation);
        return id;
    }

    public async Task<List<GameDto>> GetGamesAsync(int page, int pageSize, CancellationToken cancellation = default)
    {
        var games = await _gamesRepository.GetGamesAsync(page, pageSize, cancellation);

        return games.Select(g => g.ToDto()).ToList();
    }

    public async Task<GameFieldDto> GetGameBoardByIdAsync(int gameId, CancellationToken cancellation = default)
    {

        var board = await _gamesRepository.GetGameBoardById(gameId, cancellation);
        if (board == null)
            throw new NotFoundException($"Game with id {gameId} was not found.");

        return board.ToBoardDto();
    }
}
