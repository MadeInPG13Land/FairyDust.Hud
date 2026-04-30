namespace FairyDust.Hud.Game.Services;

internal static class CrosshairHudVisibilityService
{
    private static bool crosshairEnabled = true;

    public static bool CrosshairEnabled => crosshairEnabled;

    public static void RememberCrosshairEnabled(bool enabled) => crosshairEnabled = enabled;

    public static void Reset() => crosshairEnabled = true;
}
