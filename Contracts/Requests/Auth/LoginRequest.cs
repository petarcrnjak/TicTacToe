using System.ComponentModel.DataAnnotations;

namespace TicTacToe.Contracts.Requests.Auth
{
    public sealed record LoginRequest
    {
        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }
}