public static class BoardExtensions
{
    public static IEnumerable<string> GetRow(this string[,] board, int row)
    {
        for (int col = 0; col < board.GetLength(1); col++)
            yield return board[row, col];
    }
}