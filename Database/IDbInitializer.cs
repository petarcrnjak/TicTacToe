namespace TicTacToe.Database
{
    public interface IDbInitializer
    {
        Task InitializeAsync();
    }
}