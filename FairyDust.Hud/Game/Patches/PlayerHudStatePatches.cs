using FairyDust.Hud.Game.Services;
using HarmonyLib;
using Il2Cppmadeinfairyland.forsakenfrontiers.actor.player;

namespace FairyDust.Hud.Game.Patches;

[HarmonyPatch(typeof(FFPlayer))]
internal static class PlayerHudStatePatches
{
    [HarmonyPostfix]
    [HarmonyPatch(nameof(FFPlayer.ObservedIncapacitate))]
    private static void ObservedIncapacitatePostfix(FFPlayer __instance)
    {
        PlayerHudStateService.RememberIncapacitated(__instance);
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(FFPlayer.ObservedNoLongerIncapacitated))]
    private static void ObservedNoLongerIncapacitatedPostfix(FFPlayer __instance)
    {
        PlayerHudStateService.RememberNoLongerIncapacitated(__instance);
    }
}
