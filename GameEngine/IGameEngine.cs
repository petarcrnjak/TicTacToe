using TicTacToe.Enums;

namespace TicTacToe.GameEngine;

public interface IGameEngine
{
    bool CheckForWinner(string[] updateBoard, Player playerMarker);
    string BoardToString(string[] updateBoard);
    bool IsBoardFull(string[] updateBoard);
    string[] NormalizeBoard(string board);
    bool IsMoveValid(string[] boardArray, int row, int col);
    string[] ApplyMove(string[] boardArray, Player playerMarker, int row, int col);
}
