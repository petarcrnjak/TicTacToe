using TicTacToe.Enums;

namespace TicTacToe.Contracts.Dtos;

public sealed record GameStatusDto
{
    public int Id { get; init; }
    public string PlayerX { get; init; } = string.Empty;
    public string PlayerO { get; init; } = string.Empty;
    public string[] Board { get; init; } = new string[9];
    public string NextTurn { get; init; } = string.Empty;
    public string? Winner { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? StartedAt { get; init; }
    public string Status { get; init; } = GameStatus.Open.ToString();
}