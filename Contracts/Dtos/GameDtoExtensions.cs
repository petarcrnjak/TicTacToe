using TicTacToe.Database.Models;
using TicTacToe.Enums;
using TicTacToe.Extensions;

namespace TicTacToe.Contracts.Dtos;

public static class GameDtoExtensions
{
    public static GameDto ToDto(this Games g) =>
        new()
        {
            Id = g.Id,
            PlayerX = g.PlayerX,
            PlayerO = g.PlayerO,
            NextTurn = g.NextTurn,
            Winner = g.Winner,
            CreatedAt = g.CreatedAt,
            Status = g.Status
        };

    public static bool TryGetPlayerAndOpponent(this GameDto dto, string currentUserId, out Player playerMarker, out string opponentId)
    {
        if (dto is null)
            throw new ArgumentNullException(nameof(dto));

        playerMarker = default;
        opponentId = string.Empty;

        if (string.IsNullOrEmpty(currentUserId))
            return false;

        if (string.Equals(dto.PlayerX, currentUserId, StringComparison.OrdinalIgnoreCase))
        {
            playerMarker = Player.X;
            opponentId = dto.PlayerO;
            return true;
        }

        if (string.Equals(dto.PlayerO, currentUserId, StringComparison.OrdinalIgnoreCase))
        {
            playerMarker = Player.O;
            opponentId = dto.PlayerX;
            return true;
        }

        return false;
    }
}

public static class FieldStatusDtoExtensions
{
    public static GameFieldDto ToBoardDto(this Games g) =>
        new()
        {
            Board = g.Board.ToDisplayBoard(),
            NextTurn = g.NextTurn
        };
}
