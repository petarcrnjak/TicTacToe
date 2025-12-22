using TicTacToe.Enums;

namespace TicTacToe.Contracts.Requests;

public sealed record GamesFilter
{
    public string? UserId { get; init; }
    public DateTime? StartedFromUtc { get; init; }
    public DateTime? StartedToUtc { get; init; }
    public GameStatus? Status { get; init; }
}