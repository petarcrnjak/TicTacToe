using TicTacToe.Enums;

namespace TicTacToe.Contracts.Dtos;

public sealed record GameDto
{
    public int Id { get; init; }
    public string PlayerX { get; init; } = string.Empty;
    public string PlayerO { get; init; } = string.Empty;
    public string Winner { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public string Status { get; init; } = GameStatus.Open.ToString();
}