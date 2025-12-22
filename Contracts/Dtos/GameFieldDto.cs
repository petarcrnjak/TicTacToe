namespace TicTacToe.Contracts.Dtos;

public sealed record GameFieldDto
{
    public string[] Board { get; init; } = new string[9];
    public string NextTurn { get; init; } = string.Empty;
}
