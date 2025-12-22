using TicTacToe.Contracts.Dtos;
using TicTacToe.Database.Models;

namespace TicTacToe.Services;

public interface IPlayerStatsService
{
    Task UpdatePlayerStatsAsync(GameDto original, Games moved);
}