using TicTacToe.Enums;

namespace TicTacToe.Contracts.Requests;

public sealed record MakeMoveRequest
{
    public int GameId { get; init; }
    public string PlayerMaker { get; init; } = string.Empty;
    public string PlayerOpponent { get; init; } = string.Empty;
    public string NextTurn { get; init; } = string.Empty;
    public string BoardString { get; init; } = string.Empty;
    public string? Winner { get; init; }
    public bool BoardFull { get; init; } = false;
    public GameStatus Status { get; init; } = GameStatus.InProgress;
}
