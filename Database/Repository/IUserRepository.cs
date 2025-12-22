
using TicTacToe.Authorization;

namespace TicTacToe.Database.Repository;

public interface IUserRepository
{
    Task<AppUser?> GetUserById(string id, CancellationToken cancellation);
}
