using Il2Cppmadeinfairyland.fairyengine;
using Il2Cppmadeinfairyland.forsakenfrontiers.actor.player;

namespace FairyDust.Hud.Game.Services;

/// <summary>Local <see cref="FFPlayer"/> from <see cref="FairyEngine.LocalPlayer"/> (Il2Cpp cast).</summary>
internal static class FairyLocalPlayer
{
    public static FFPlayer Current => FairyEngine.LocalPlayer?.TryCast<FFPlayer>();
}
