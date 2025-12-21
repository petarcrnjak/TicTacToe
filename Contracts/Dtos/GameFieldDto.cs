using TicTacToe.Enums;

namespace TicTacToe.Contracts.Dtos;

public sealed record GameFieldDto
{
    public string Board { get; init; } = string.Empty;
}
