namespace TicTacToe.Contracts;

public sealed record GameMove
{
    public int Row { get; init; }
    public int Col { get; init; }
}
