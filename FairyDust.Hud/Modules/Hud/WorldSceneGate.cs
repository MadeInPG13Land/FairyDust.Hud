using UnityEngine.SceneManagement;

namespace FairyDust.Hud.Modules.Hud;

/// <summary>Tracks loaded scene name and exposes gameplay visibility from the active scene (not a stale cache).</summary>
internal static class WorldSceneGate
{
    public const string ForsakenWorldSceneName = "Forsaken Frontiers";

    private static string activeSceneName = string.Empty;

    public static void OnSceneWasLoaded(int buildIndex, string sceneName)
    {
        activeSceneName = sceneName ?? string.Empty;
    }

    public static bool IsForsakenWorldLoaded =>
        string.Equals(activeSceneName, ForsakenWorldSceneName, StringComparison.Ordinal);

    public static bool IsActiveGameplayScene =>
        string.Equals(SceneManager.GetActiveScene().name, ForsakenWorldSceneName, StringComparison.Ordinal);
}
