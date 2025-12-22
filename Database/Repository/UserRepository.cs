
using Dapper;
using System.Data.Common;
using TicTacToe.Authorization;

namespace TicTacToe.Database.Repository;

public sealed class UserRepository : IUserRepository
{
    private readonly IDbConnectionFactory _connectionFactory;
    public UserRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<AppUser?> GetUserById(string id, CancellationToken cancellation)
    {
        await using DbConnection connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellation);

        return await connection.QuerySingleOrDefaultAsync<AppUser>(@"
               SELECT UserName, Wins, GamesPlayed
               FROM AspNetUsers
               WHERE Id=@UserId",
                new { UserId = id });
    }
}
