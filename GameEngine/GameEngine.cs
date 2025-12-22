using TicTacToe.Enums;

namespace TicTacToe.GameEngine;

public class GameEngine : IGameEngine
{
    public bool CheckForWinner(string[] updateBoard, Player playerMarker)
    {
        var markerName = playerMarker.ToString();
        var markerNumeric = ((int)playerMarker).ToString();
        // Check rows
        int[][] lines =
        [
            [0,1,2],
            [3,4,5],
            [6,7,8],
            [0,3,6],
            [1,4,7],
            [2,5,8],
            [0,4,8],
            [2,4,6]
        ];

        foreach (var line in lines)
        {
            var a = updateBoard[line[0]]?.Trim();
            var b = updateBoard[line[1]]?.Trim();
            var c = updateBoard[line[2]]?.Trim();

            if (a is null || b is null || c is null) continue;

            bool aMatches = a == markerName || a == markerNumeric;
            bool bMatches = b == markerName || b == markerNumeric;
            bool cMatches = c == markerName || c == markerNumeric;

            if (aMatches && bMatches && cMatches)
                return true;
        }

        return false;
    }

    public bool IsMoveValid(string[] board, int row, int col)
    {
        if (board is null || board.Length != 9)
            return false;

        if (row < 0 || row > 2 || col < 0 || col > 2)
            return false;

        var index = row * 3 + col;
        return string.Equals(board[index]?.Trim(), "0", StringComparison.OrdinalIgnoreCase);
    }

    public string[] ApplyMove(string[] board, Player player, int row, int col)
    {
        if (!IsMoveValid(board, row, col))
            throw new InvalidOperationException("Move is invalid.");

        var result = (string[])board.Clone();
        result[row * 3 + col] = ((int)player).ToString();
        return result;
    }

    public string BoardToString(string[] board)
    {
        return string.Join(",", board);
    }

    public bool IsBoardFull(string[] board)
    {
        if (board is null || board.Length != 9)
            return false;

        return board.All(c => !string.IsNullOrWhiteSpace(c) && c != "0");
    }

    public string[] NormalizeBoard(string csv)
    {
        var board = (csv ?? string.Join(',', Enumerable.Repeat("0", 9)))
            .Split(',', StringSplitOptions.None)
            .Select(p => string.IsNullOrWhiteSpace(p) ? "0" : p.Trim())
            .ToArray();

        if (board.Length != 9)
            throw new ArgumentException("Board must contain 9 cells.", nameof(csv));

        return board;
    }
}
