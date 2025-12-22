namespace TicTacToe.Contracts.Dtos;

public sealed record UserDto
{
    public string? UserName { get; init; }
    public int Wins { get; init; }
    public int GamesPlayed { get; init; }
}
