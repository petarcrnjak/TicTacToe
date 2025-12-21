using TicTacToe.Enums;

namespace TicTacToe.Contracts.Requests
{
    public sealed record CreateGameRequest
    {
        public Guid UserId { get; init;}
        public DateTime CreatedUtc { get; init;} = DateTime.UtcNow; 
        public GameStatus Status { get; init;} = GameStatus.Open;
    }
}
