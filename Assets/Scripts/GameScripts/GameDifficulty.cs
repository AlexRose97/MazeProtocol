namespace GameScripts
{
    public enum DifficultyLevel
    {
        Easy,
        Medium,
        Hard
    }

    public static class GameDifficulty
    {
        public static DifficultyLevel Current { get; private set; } = DifficultyLevel.Easy;

        public static void SetDifficulty(DifficultyLevel level)
        {
            Current = level;
        }
    }
}