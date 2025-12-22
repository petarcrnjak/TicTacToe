namespace TicTacToe.Contracts.Dtos;

public sealed record UserDto
{
    public string? UserName { get; init; }
    public int Wins { get; init; }
    public int GamesPlayed { get; init; }
    public double WinPercentage { get; init; } = 0.0;
}
