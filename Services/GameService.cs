using TicTacToe.Authorization;
using TicTacToe.Contracts;
using TicTacToe.Contracts.Dtos;
using TicTacToe.Contracts.Requests;
using TicTacToe.Database.Exceptions;
using TicTacToe.Database.Repository;
using TicTacToe.Enums;
using TicTacToe.GameEngine;
using TicTacToe.GameEngine.Validators;

namespace TicTacToe.Services;

public sealed class GameService : IGameService
{
    private readonly IGamesRepository _gamesRepository;
    private readonly IUserContext _userContext;
    private readonly IGameEngine _gameEngine;
    private readonly IPlayerStatsService _playerStatsService;

    public GameService(IGamesRepository gamesRepository, IUserContext userContext, IGameEngine gameEngine, IPlayerStatsService playerStatsService)
    {
        _gamesRepository = gamesRepository;
        _userContext = userContext;
        _gameEngine = gameEngine;
        _playerStatsService = playerStatsService;
    }

    public async Task<int?> StartGameAsync(CancellationToken cancellation = default)
    {
        var userId = GetCurrentUserId();

        var newGame = new CreateGameRequest
        {
            UserId = userId,
            CreatedUtc = DateTime.UtcNow,
            Status = GameStatus.Open
        };

        var id = await _gamesRepository.CreateGameAsync(newGame, cancellation);
        return id;
    }

    public async Task<List<GameViewDto>> GetGamesAsync(int page, int pageSize, CancellationToken cancellation = default)
    {
        var games = await _gamesRepository.GetGamesAsync(page, pageSize, cancellation);
        return games.Select(g => g.ToViewDto()).ToList();
    }

    public async Task<List<GameFilterDto>> GetGamesFilterAsync(GamesFilter filter, int page, int pageSize, CancellationToken cancellation = default)
    {
        var games = await _gamesRepository.GetGamesFilteredAsync(filter, page, pageSize, cancellation);
        return games.Select(g => g.ToFilterViewDto()).ToList();
    }

    public async Task<GameFieldDto> GetGameBoardByIdAsync(int gameId, CancellationToken cancellation = default)
    {
        var board = await _gamesRepository.GetGameBoardByIdOrThrowAsync(gameId, cancellation);
        return board.ToBoardDto();
    }

    public async Task<GameStatusDto> JoinGameAsync(int gameId, CancellationToken cancellation = default)
    {
        var game = await _gamesRepository.GetOpenGameByIdOrThrowAsync(gameId, cancellation, "Game with id {0} was not found or is not joinable.");

        var gameDto = game.ToDto();
        var currentUserId = _userContext.UserId.ToString();

        if (gameDto.PlayerX == currentUserId)
            throw new InvalidOperationException("Cannot join your own game.");

        var joined = await _gamesRepository.JoinGameAsync(gameId, gameDto.PlayerX, currentUserId, cancellation);
        if (!joined)
            throw new InvalidOperationException("Unable to join the game (it may have been taken).");

        var updated = await _gamesRepository.GetGameByIdOrThrowAsync(gameId, cancellation, "Game with id {0} was not found after join.");
        return updated.ToStatusDto();
    }

    public async Task<GameStatusDto> PlayGameAsync(int gameId, GameMove requestedMove, CancellationToken cancellation = default)
    {
        var currentUserId = GetCurrentUserId().ToString();

        var game = await _gamesRepository.GetGameByIdOrThrowAsync(gameId, cancellation);
        var gameDto = game.ToDto();

        GameValidator.ValidateGameTurn(gameDto, currentUserId, gameId);

        if (string.IsNullOrEmpty(gameDto.Board))
            throw new NotFoundException($"Board for game {gameId} was not found.");

        var boardArray = _gameEngine.NormalizeBoard(gameDto.Board);
        if (!_gameEngine.IsMoveValid(boardArray, requestedMove.Row, requestedMove.Col))
            throw new InvalidOperationException("The move is not allowed.");

        if (!gameDto.TryGetPlayerAndOpponent(currentUserId, out var playerMarker, out var opponentId))
            throw new InvalidOperationException("Not a player in this game.");

        var updateBoard = _gameEngine.ApplyMove(boardArray, playerMarker, requestedMove.Row, requestedMove.Col);
        var hasWinner = _gameEngine.CheckForWinner(updateBoard, playerMarker);

        var newMoveRequest = new MakeMoveRequest
        {
            GameId = gameId,
            PlayerMaker = currentUserId,
            PlayerOpponent = opponentId,
            NextTurn = hasWinner ? string.Empty : opponentId,
            BoardString = _gameEngine.BoardToString(updateBoard),
            Winner = hasWinner ? currentUserId : null,
            BoardFull = hasWinner ? true : _gameEngine.IsBoardFull(updateBoard),
            Status = hasWinner || _gameEngine.IsBoardFull(updateBoard) ? GameStatus.Finished : GameStatus.InProgress
        };

        var moved = await _gamesRepository.MakeMoveAsync(newMoveRequest, cancellation);
        if (moved == null)
            throw new InvalidOperationException("Unable to make the move (it may be invalid).");

        await _playerStatsService.UpdatePlayerStatsAsync(gameDto, moved);

        return moved.ToStatusDto();
    }

    private Guid GetCurrentUserId() => _userContext.UserId;
}
