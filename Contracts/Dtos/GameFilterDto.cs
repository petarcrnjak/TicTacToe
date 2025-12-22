using TicTacToe.Enums;

namespace TicTacToe.Contracts.Dtos;

public sealed record GameFilterDto
{
    public int Id { get; init; }
    public string PlayerX { get; init; } = string.Empty;
    public string PlayerO { get; init; } = string.Empty;
    public string[] Board { get; set; } = new string[9];
    public string NextTurn { get; init; } = string.Empty;
    public string? Winner { get; init; }
    public DateTime CreatedAt { get; init; }
    public string Status { get; init; } = GameStatus.Open.ToString();
}
