using TicTacToe.Enums;

namespace TicTacToe.Extensions
{
    public static class BoardExtensions
    {
        // Convert stored board (e.g. "0,1,0,0,2,0,0,0,0") -> display board where "0" becomes ""
        public static string[] ToDisplayBoard(this string? storedBoard)
        {
            var parts = (storedBoard ?? string.Join(',', Enumerable.Repeat("0", 9)))
                .Split(',', StringSplitOptions.None);

            if (parts.Length != 9)
                throw new ArgumentException("Board must contain 9 comma-separated cells.", nameof(storedBoard));

            return parts.Select(p =>
            {
                return p switch
                {
                    "0" => string.Empty,
                    "1" => Player.X.ToString(),
                    "2" => Player.O.ToString(),
                    _ => p // unexpected values preserved
                };
            }).ToArray();
        }
    }
}