using System.Data.Common;

namespace TicTacToe.Database;

public interface IDbConnectionFactory
{
    DbConnection CreateConnection();
}
