using Il2CppInterop.Runtime;
using Il2Cppmadeinfairyland.forsakenfrontiers;
using Il2Cppmadeinfairyland.forsakenfrontiers.actor.player;
using Il2Cppmadeinfairyland.forsakenfrontiers.actor.player.datadeck;
using UnityEngine;

namespace FairyDust.Hud.Game.Services;

internal static class DataDeckFlavorService
{
    private static readonly Dictionary<IntPtr, Color> colorsByPlayer = new();

    public static bool TryReadFlavorColor(FFDataDeck deck, out Color color)
    {
        color = default;
        if (deck == null)
        {
            return false;
        }

        FFPlayer player;
        try
        {
            player = deck.Player;
        }
        catch
        {
            return false;
        }

        return TryReadFlavorColor(player, out color);
    }

    public static bool TryReadFlavorColor(FFPlayer player, out Color color)
    {
        color = default;
        if (player == null)
        {
            return false;
        }

        IntPtr playerPointer = IL2CPP.Il2CppObjectBaseToPtr(player);
        if (playerPointer != IntPtr.Zero && colorsByPlayer.TryGetValue(playerPointer, out Color cachedColor))
        {
            color = cachedColor;
            return true;
        }

        try
        {
            color = player.Flavor.color;
            color.a = 1f;
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static void RememberFlavor(FFPlayer player, FFGameplayStatics.FlavorData flavor)
    {
        if (!IsCurrentPlayer(player))
        {
            return;
        }

        Color color = flavor.color;
        color.a = 1f;
        colorsByPlayer[IL2CPP.Il2CppObjectBaseToPtr(player)] = color;
    }

    public static void Clear() => colorsByPlayer.Clear();

    private static bool IsCurrentPlayer(FFPlayer player)
    {
        if (player == null)
        {
            return false;
        }

        FFPlayer current = FairyLocalPlayer.Current;
        if (current == null)
        {
            return false;
        }

        return IL2CPP.Il2CppObjectBaseToPtr(player) == IL2CPP.Il2CppObjectBaseToPtr(current);
    }
}
