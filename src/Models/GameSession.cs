public class GameSession
{
    public string OpponentName { get; set; } = "";
    public bool IsMyTurn { get; set; } = false;
    
    public string[,] OpponentBoard { get; private set; }
    public string[,] PlayerBoard { get; private set; }
    public List<string> AttackHistory { get; set; }

    private const int BOARD_SIZE = 10;

    public GameSession()
    {
        OpponentBoard = InitEmptyBoard();
        PlayerBoard = InitEmptyBoard();
        AttackHistory = new List<string>();
    }

    public string[,] InitEmptyBoard()
    {
        var board = new string[BOARD_SIZE, BOARD_SIZE];
        for (int i = 0; i < BOARD_SIZE; i++)
        for (int j = 0; j < BOARD_SIZE; j++)
            board[i, j] = "[blue]⏹[/]";

        return board;
    }

    public void SetPlayerBoard(string[,] board)
    {
        PlayerBoard = board;
    }

    public void SetOpponentBoard(string[,] board)
    {
        OpponentBoard = board;
    }
}