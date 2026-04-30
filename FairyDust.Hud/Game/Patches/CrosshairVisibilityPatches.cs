using FairyDust.Hud.Game.Services;
using HarmonyLib;
using Il2Cppmadeinfairyland.forsakenfrontiers.ui.PauseMenu.OptionsMenu.Gameplay;

namespace FairyDust.Hud.Game.Patches;

[HarmonyPatch(typeof(FFToggleCrosshair))]
internal static class FFToggleCrosshairVisibilityPatches
{
    [HarmonyPostfix]
    [HarmonyPatch(nameof(FFToggleCrosshair.ToggledOn))]
    private static void ToggledOnPostfix()
    {
        CrosshairHudVisibilityService.RememberCrosshairEnabled(true);
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(FFToggleCrosshair.ToggledOff))]
    private static void ToggledOffPostfix()
    {
        CrosshairHudVisibilityService.RememberCrosshairEnabled(false);
    }
}

[HarmonyPatch(typeof(FFToggleCrosshairMessage))]
internal static class FFToggleCrosshairMessageVisibilityPatches
{
    [HarmonyPostfix]
    [HarmonyPatch(nameof(FFToggleCrosshairMessage.ToggledOn))]
    private static void ToggledOnPostfix()
    {
        CrosshairHudVisibilityService.RememberCrosshairEnabled(true);
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(FFToggleCrosshairMessage.ToggledOff))]
    private static void ToggledOffPostfix()
    {
        CrosshairHudVisibilityService.RememberCrosshairEnabled(false);
    }
}
