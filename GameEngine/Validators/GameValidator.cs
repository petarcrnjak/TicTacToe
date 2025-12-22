using TicTacToe.Contracts.Dtos;
using TicTacToe.Database.Exceptions;
using TicTacToe.Enums;

namespace TicTacToe.GameEngine.Validators;

internal static class GameValidator
{
    public static void ValidateGameTurn(GameDto gameDto, string currentUserId, int gameId)
    {
        if (gameDto.Status != GameStatus.InProgress || gameDto.NextTurn != currentUserId)
            throw new NotFoundException($"Game with id {gameId} is not started or it's not your turn");
    }
}