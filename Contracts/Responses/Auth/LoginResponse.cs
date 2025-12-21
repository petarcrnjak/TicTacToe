namespace TicTacToe.Contracts.Responses.Auth
{
    public sealed record LoginResponse
    {
        public string Token { get; init; } = string.Empty;
        public string Username { get; init; } = string.Empty;
    }
}