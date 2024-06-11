using System.Runtime.InteropServices;

public static class FirebaseUtils
{
    /// <summary>
    /// Get an array of the top 10 players in the leaderboard
    /// </summary>
    [DllImport("__Internal")]
    public static extern void GetLeaderboard();

    /// <summary>
    /// Update user an array of the top 10 players in the leaderboard
    /// </summary>
    [DllImport("__Internal")]
    public static extern void UpdateUserHighScore(int highscore);
}