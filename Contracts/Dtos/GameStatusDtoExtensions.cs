using TicTacToe.Database.Models;
using TicTacToe.Extensions;

namespace TicTacToe.Contracts.Dtos;

public static class GameStatusDtoExtensions
{
    public static GameStatusDto ToStatusDto(this Games g) =>
        new()
        {
            Id = g.Id,
            PlayerX = g.PlayerX,
            PlayerO = g.PlayerO,
            Board = g.Board.ToDisplayBoard(),
            NextTurn = g.NextTurn,
            Winner = g.Winner,
            CreatedAt = g.CreatedAt,
            StartedAt = g.StartedAt == default ? null : g.StartedAt,
            Status = g.Status.ToString()
        };
}