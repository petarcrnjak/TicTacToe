using TicTacToe.Authorization;

namespace TicTacToe.Contracts.Dtos;

public static class UserDtoExtensions
{
    public static UserDto ToDto(this AppUser user) =>
        new()
        {
            UserName = user.UserName,
            Wins = user.Wins,
            GamesPlayed = user.GamesPlayed,
            WinPercentage = CalculateWinPercentage(user.Wins, user.GamesPlayed)
        };

    private static double CalculateWinPercentage(int wins, int gamesPlayed)
    {
        if (gamesPlayed <= 0)
            return 0.0;
        var raw = (double)wins * 100.0 / gamesPlayed;

        return Math.Round(raw, 2, MidpointRounding.AwayFromZero);
    }
}
