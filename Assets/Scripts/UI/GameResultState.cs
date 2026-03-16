public enum GameResult
{
    None,
    Win,
    Lose
}

public static class GameResultState
{
    public static GameResult LastResult = GameResult.None;

    public static void Clear()
    {
        LastResult = GameResult.None;
    }
}