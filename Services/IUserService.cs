using TicTacToe.Contracts.Dtos;

namespace TicTacToe.Services;

public interface IUserService
{
    Task<UserDto> GetUserByIdAsync(string id, CancellationToken cancellation = default);
}
