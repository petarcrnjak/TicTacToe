using Microsoft.AspNetCore.Identity;

namespace TicTacToe.Authorization
{
    public class AppUser : IdentityUser
    {
        public int GamesPlayed { get; set; }
        public int Wins { get; set; }
    }
}