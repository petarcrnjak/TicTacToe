using System.ComponentModel.DataAnnotations;

namespace TicTacToe.Contracts.Requests.Auth
{
    public sealed record RegisterRequest
    {
        [Required]
        public string? Username { get; set; } = string.Empty;

        [Required]
        public string? Password { get; set; } = string.Empty;
    }
}