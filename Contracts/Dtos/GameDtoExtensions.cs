using TicTacToe.Database.Models;

namespace TicTacToe.Contracts.Dtos;

public static class GameDtoExtensions
{
    public static GameDto ToDto(this Games g) =>
        new()
        {
            Id = g.Id,
            PlayerX = g.PlayerX,
            PlayerO = g.PlayerO,
            Winner = g.Winner,
            CreatedAt = g.CreatedAt,
            Status = g.Status.ToString()
        };
}

public static class FieldStatusDtoExtensions
{
    public static GameFieldDto ToBoardDto(this Games g) =>
        new()
        {
            Board = g.Board,
        };
}
