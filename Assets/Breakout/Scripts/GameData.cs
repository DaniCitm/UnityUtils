public static class GameData
{
    public static int score;

    public static void ResetScore()
	{
		score = 0; 
	}

	public static void ResetGame()
	{
		ResetScore();
	}
}
