using TicTacToe.Contracts.Dtos;
using TicTacToe.Database.Repository;

namespace TicTacToe.Services;

public sealed class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserDto> GetUserByIdAsync(string id, CancellationToken cancellation = default)
    {
        var user = await _userRepository.GetUserById(id, cancellation);
        return user == null ? throw new InvalidOperationException("User not found") : user.ToDto();
    }
}
