using Il2Cppmadeinfairyland.forsakenfrontiers.actor.player;
using UnityEngine;

namespace FairyDust.Hud.Modules.Status.Services;

internal sealed class StatusFormatterService
{
    private readonly PlayerStatusReadService reads;

    public StatusFormatterService(PlayerStatusReadService reads)
    {
        this.reads = reads;
    }

    public string Timer(float seconds)
    {
        int totalSeconds = Mathf.Max(0, Mathf.CeilToInt(seconds));
        int minutes = totalSeconds / 60;
        int secs = totalSeconds % 60;
        return $"{minutes:00}:{secs:00}";
    }

    public string FrostbiteValue(FFPlayer player, float frostbite, float maxFrostbite)
    {
        float speed = reads.FrostbiteSpeed(player);
        bool active = reads.IsFrostbiteActive(player);
        if (active && speed > 0.001f && frostbite < maxFrostbite)
        {
            return Timer((maxFrostbite - frostbite) / speed);
        }

        float ratio = frostbite / Mathf.Max(maxFrostbite, 1e-5f);
        return Mathf.RoundToInt(Mathf.Clamp01(ratio) * 100f) + "%";
    }
}
