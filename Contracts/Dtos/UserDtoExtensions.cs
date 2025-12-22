using TicTacToe.Authorization;

namespace TicTacToe.Contracts.Dtos;

public static class UserDtoExtensions
{
    public static UserDto ToDto(this AppUser user) =>
        new()
        {
            UserName = user.UserName,
            Wins = user.Wins,
            GamesPlayed = user.GamesPlayed
        };
}
