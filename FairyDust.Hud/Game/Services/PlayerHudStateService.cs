using Il2CppInterop.Runtime;
using Il2Cppmadeinfairyland.forsakenfrontiers.actor.player;

namespace FairyDust.Hud.Game.Services;

internal static class PlayerHudStateService
{
    private static IntPtr incapacitatedPlayer;

    public static bool IsCurrentPlayerIncapacitated => IsCurrentPlayer(incapacitatedPlayer);

    public static void RememberIncapacitated(FFPlayer player)
    {
        if (IsCurrentPlayer(player))
        {
            incapacitatedPlayer = IL2CPP.Il2CppObjectBaseToPtr(player);
        }
    }

    public static void RememberNoLongerIncapacitated(FFPlayer player)
    {
        if (IsCurrentPlayer(player))
        {
            incapacitatedPlayer = IntPtr.Zero;
        }
    }

    public static void Reset() => incapacitatedPlayer = IntPtr.Zero;

    private static bool IsCurrentPlayer(IntPtr playerPointer)
    {
        if (playerPointer == IntPtr.Zero)
        {
            return false;
        }

        FFPlayer current = FairyLocalPlayer.Current;
        return current != null && IL2CPP.Il2CppObjectBaseToPtr(current) == playerPointer;
    }

    private static bool IsCurrentPlayer(FFPlayer player)
    {
        if (player == null)
        {
            return false;
        }

        return IsCurrentPlayer(IL2CPP.Il2CppObjectBaseToPtr(player));
    }
}
