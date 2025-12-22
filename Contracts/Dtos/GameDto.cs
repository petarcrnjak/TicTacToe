using TicTacToe.Enums;

namespace TicTacToe.Contracts.Dtos;

public sealed record GameDto
{
    public int Id { get; init; }
    public string PlayerX { get; init; } = string.Empty;
    public string PlayerO { get; init; } = string.Empty;
    public string Board { get; set; } = BoardExtensions.EmptyBoardCsv;
    public string NextTurn { get; init; } = string.Empty;
    public string? Winner { get; init; }
    public DateTime CreatedAt { get; init; }
    public GameStatus Status { get; init; } = GameStatus.Open;

}