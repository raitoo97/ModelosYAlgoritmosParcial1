public static class GameState
{
    public static bool IsPaused { get; private set; }
    public static void Pause()
    {
        IsPaused = true;
    }
    public static void Resume()
    {
        IsPaused = false;
    }
}
