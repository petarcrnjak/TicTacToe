using TicTacToe.Enums;

namespace TicTacToe.Database.Models;

public class Games
{
    public int Id { get; set; }
    public string PlayerX { get; set; } = string.Empty;
    public string PlayerO { get; set; } = string.Empty;
    public string Board { get; set; } = "0,0,0,0,0,0,0,0,0";
    public string Winner { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime StartedAt { get; set; }
    public GameStatus Status { get; set; } = GameStatus.Open;

}
