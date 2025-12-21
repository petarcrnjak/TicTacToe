using System;
using System.Data.Common;
using Microsoft.Data.Sqlite;

namespace TicTacToe.Database
{
    public sealed class SqliteConnectionFactory : IDbConnectionFactory
    {
        private readonly string _connectionString;

        public SqliteConnectionFactory(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        public DbConnection CreateConnection() => new SqliteConnection(_connectionString);
    }
}