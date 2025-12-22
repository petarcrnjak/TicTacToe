using Microsoft.AspNetCore.Identity;
using TicTacToe.Authorization;
using TicTacToe.Contracts.Dtos;
using TicTacToe.Database.Models;
using TicTacToe.Enums;

namespace TicTacToe.Services;

internal sealed class PlayerStatsService : IPlayerStatsService
{
    private readonly UserManager<AppUser> _userManager;

    public PlayerStatsService(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task UpdatePlayerStatsAsync(GameDto original, Games moved)
    {
        var originalBoard = original.Board ?? BoardExtensions.EmptyBoardCsv;
        var originalStatus = original.Status;

        var isFirstMove = string.Equals(originalBoard, BoardExtensions.EmptyBoardCsv, StringComparison.Ordinal);

        if (isFirstMove)
        {
            if (!string.IsNullOrWhiteSpace(moved.PlayerX))
                await ModifyUserAsync(moved.PlayerX, u => u.GamesPlayed += 1);

            if (!string.IsNullOrWhiteSpace(moved.PlayerO))
                await ModifyUserAsync(moved.PlayerO, u => u.GamesPlayed += 1);
        }

        if (originalStatus != GameStatus.Finished && moved.Status == GameStatus.Finished)
        {
            if (!string.IsNullOrWhiteSpace(moved.Winner))
                await ModifyUserAsync(moved.Winner!, u => u.Wins += 1);
        }
    }

    private async Task ModifyUserAsync(string userId, Action<AppUser> apply)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null) return;

        apply(user);
        await _userManager.UpdateAsync(user);
    }
}