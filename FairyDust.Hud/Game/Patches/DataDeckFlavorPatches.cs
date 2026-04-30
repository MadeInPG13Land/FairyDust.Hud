using FairyDust.Hud.Game.Services;
using HarmonyLib;
using Il2Cppmadeinfairyland.forsakenfrontiers;
using Il2Cppmadeinfairyland.forsakenfrontiers.actor.player;
using Il2Cppmadeinfairyland.forsakenfrontiers.actor.player.datadeck;

namespace FairyDust.Hud.Game.Patches;

[HarmonyPatch(typeof(FFPlayer))]
internal static class FFPlayerFlavorPatches
{
    [HarmonyPostfix]
    [HarmonyPatch("obr_FlavorSynced")]
    private static void FlavorSyncedPostfix(FFPlayer __instance, FFGameplayStatics.FlavorData flavor)
    {
        DataDeckFlavorService.RememberFlavor(__instance, flavor);
    }
}

[HarmonyPatch(typeof(FFDataDeck))]
internal static class FFDataDeckFlavorPatches
{
    [HarmonyPostfix]
    [HarmonyPatch(nameof(FFDataDeck.ApplyFlavor))]
    private static void ApplyFlavorPostfix(FFDataDeck __instance, FFGameplayStatics.FlavorData flavor)
    {
        DataDeckFlavorService.RememberFlavor(__instance?.Player, flavor);
    }
}
