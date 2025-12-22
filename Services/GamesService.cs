using Microsoft.AspNetCore.Identity;
using TicTacToe.Authorization;
using TicTacToe.Contracts;
using TicTacToe.Contracts.Dtos;
using TicTacToe.Contracts.Requests;
using TicTacToe.Database.Repository;
using TicTacToe.Enums;
using TicTacToe.Exceptions;
using TicTacToe.GameEngine;

namespace TicTacToe.Services;

public class GamesService : IGamesService
{
    private readonly IGamesRepository _gamesRepository;
    private readonly IUserContext _userContext;
    private readonly UserManager<AppUser> _userManager;
    private readonly IGameEngine _gameEngine;
    public GamesService(IGamesRepository gamesRepository, IUserContext userContext, UserManager<AppUser> usermanager, IGameEngine gameEngine)
    {
        _gamesRepository = gamesRepository;
        _userContext = userContext;
        _userManager = usermanager;
        _gameEngine = gameEngine;
    }

    public async Task<int?> StartGameAsync(CancellationToken cancellation = default)
    {
        var currentUserId = _userContext.UserId;
        var user = await _userManager.FindByIdAsync(currentUserId.ToString());
        if (user == null)
            throw new NotFoundException($"User with id {currentUserId} was not found.");

        var newGame = new CreateGameRequest
        {
            UserId = currentUserId,
            CreatedUtc = DateTime.UtcNow,
            Status = GameStatus.Open
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

    public async Task<GameStatusDto> JoinGameAsync(int gameId, CancellationToken cancellation = default)
    {
        var game = await _gamesRepository.GetOpenGameByIdAsync(gameId, cancellation);
        if (game == null)
            throw new NotFoundException($"Game with id {gameId} was not found or is not joinable.");

        var gameDto = game.ToDto();
        var currentUserId = _userContext.UserId.ToString();
        if (gameDto.PlayerX == currentUserId)
            throw new InvalidOperationException("Cannot join your own game.");

        var joined = await _gamesRepository.JoinGameAsync(gameId, gameDto.PlayerX, currentUserId, cancellation);
        if (!joined)
            throw new InvalidOperationException("Unable to join the game (it may have been taken).");

        var updated = await _gamesRepository.GetGameByIdAsync(gameId, cancellation);
        if (updated == null)
            throw new NotFoundException($"Game with id {gameId} was not found after join.");

        return updated.ToStatusDto();
    }

    public async Task<GameStatusDto> MakeMoveAsync(int gameId, GameMove requestedMove, CancellationToken cancellation = default)
    {
        var currentUserId = _userContext.UserId.ToString();

        var game = await _gamesRepository.GetGameByIdAsync(gameId, cancellation);
        if (game == null)
            throw new NotFoundException($"Game with id {gameId} was not found");

        var gameDto = game.ToDto();
        if (gameDto.Status != GameStatus.InProgress || gameDto.NextTurn != currentUserId)
            throw new NotFoundException($"Game with id {gameId} is not started or it's not your turn");

        var gameBoard = await _gamesRepository.GetGameBoardById(gameId, cancellation);
        if (gameBoard == null)
            throw new NotFoundException($"Board for game {gameId} was not found.");

        var boardArray = _gameEngine.NormalizeBoard(gameBoard.Board);
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

        return moved.ToStatusDto();
    }
}
