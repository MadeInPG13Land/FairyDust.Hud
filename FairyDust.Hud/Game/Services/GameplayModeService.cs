using Il2Cppmadeinfairyland.forsakenfrontiers;

namespace FairyDust.Hud.Game.Services;

internal static class GameplayModeService
{
    public static bool IsExplorationMode()
    {
        try
        {
            return FFGameplayStatics.selectedGamemode == FFGameplayStatics.GameMode.EXPLORATION;
        }
        catch
        {
            return false;
        }
    }
}
