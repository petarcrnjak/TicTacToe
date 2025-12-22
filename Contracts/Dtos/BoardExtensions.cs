using TicTacToe.Enums;

namespace TicTacToe.Contracts.Dtos
{
    public static class BoardExtensions
    {
        public const string EmptyBoardCsv = "0,0,0,0,0,0,0,0,0";

        public static string[] NormalizeBoard(this string? csv)
        {
            var parts = (csv ?? EmptyBoardCsv)
                .Split(',', StringSplitOptions.None)
                .Select(p => (p ?? string.Empty).Trim())
                .ToArray();

            if (parts.Length != 9)
                throw new ArgumentException("Board must contain 9 comma-separated cells.", nameof(csv));

            for (var i = 0; i < parts.Length; i++)
                if (string.IsNullOrEmpty(parts[i]))
                    parts[i] = "0";

            return parts;
        }

        public static string[] ToDisplayBoardParts(this string? storedBoard)
        {
            var parts = storedBoard.NormalizeBoard();
            return parts.Select(p => p switch
            {
                "0" => string.Empty,
                "1" => Player.X.ToString(),
                "2" => Player.O.ToString(),
                _ => p
            }).ToArray();
        }
    }
}