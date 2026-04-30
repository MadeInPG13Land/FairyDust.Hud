using Il2Cppmadeinfairyland.forsakenfrontiers.actor.player;
using Il2Cppmadeinfairyland.forsakenfrontiers.actor.player.datadeck;

namespace FairyDust.Hud.Modules.Status.Services;

internal sealed class PlayerStatusReadService
{
    private const float MinimumMeaningfulTimer = 0.01f;
    private const float TimerUpperBoundTolerance = 1f;

    public bool IsUsable(FFPlayer player)
    {
        if (player == null)
        {
            return false;
        }

        try
        {
            return player.gameObject != null && player.gameObject.activeInHierarchy;
        }
        catch
        {
            return false;
        }
    }

    public FFDataDeck GetDataDeck(FFPlayer player)
    {
        try
        {
            return player != null ? player.DataDeck : null;
        }
        catch
        {
            return null;
        }
    }

    public bool IsDataDeckOpen(FFDataDeck deck)
    {
        try
        {
            return deck != null && deck.DataDeckOpen;
        }
        catch
        {
            return false;
        }
    }

    public bool IsBleeding(FFPlayer player)
    {
        try
        {
            return player != null && player.IsBleeding;
        }
        catch
        {
            return false;
        }
    }

    public bool HasActiveBleedOut(FFPlayer player, out float timer, out float total)
    {
        timer = 0f;
        total = 0f;
        if (!IsBleeding(player))
        {
            return false;
        }

        total = BleedToDeathTime(player);
        timer = BleedToDeathTimer(player);
        if (total <= MinimumMeaningfulTimer || timer <= MinimumMeaningfulTimer)
        {
            return false;
        }

        if (timer > total + TimerUpperBoundTolerance)
        {
            return false;
        }

        return true;
    }

    public bool IsInfected(FFPlayer player)
    {
        try
        {
            return player != null && player.Infected;
        }
        catch
        {
            return false;
        }
    }

    public bool IsFrostbiteActive(FFPlayer player)
    {
        try
        {
            return player != null && player.FrostbiteActive;
        }
        catch
        {
            return false;
        }
    }

    public float BleedToDeathTime(FFPlayer player)
    {
        try
        {
            return player != null ? player.BleedToDeathTime : 0f;
        }
        catch
        {
            return 0f;
        }
    }

    public float BleedToDeathTimer(FFPlayer player)
    {
        try
        {
            return player != null ? player.BleedToDeathTimer : 0f;
        }
        catch
        {
            return 0f;
        }
    }

    public float InfectionTakeoverDuration(FFPlayer player)
    {
        try
        {
            return player != null ? player.InfectionTakeoverDuration : 0f;
        }
        catch
        {
            return 0f;
        }
    }

    public float InfectionTimer(FFPlayer player)
    {
        try
        {
            return player != null ? player.InfectionTimer : 0f;
        }
        catch
        {
            return 0f;
        }
    }

    public float MaxFrostbite(FFPlayer player)
    {
        try
        {
            return player != null ? player.maxFrostbite : 0f;
        }
        catch
        {
            return 0f;
        }
    }

    public float Frostbite(FFPlayer player)
    {
        try
        {
            return player != null ? player.Frostbite : 0f;
        }
        catch
        {
            return 0f;
        }
    }

    public float FrostbiteSpeed(FFPlayer player)
    {
        try
        {
            return player != null ? player.frostbiteSpeed : 0f;
        }
        catch
        {
            return 0f;
        }
    }

    public bool NearWarmth(FFPlayer player)
    {
        try
        {
            return player != null && player.NearWarmth;
        }
        catch
        {
            return false;
        }
    }

    public bool HasWarmthBenefits(FFPlayer player)
    {
        try
        {
            return player != null && player.HasWarmthBenefits;
        }
        catch
        {
            return false;
        }
    }

    public float MaxStamina(FFPlayer player)
    {
        try
        {
            return player != null ? player.MaxStamina : 0f;
        }
        catch
        {
            return 0f;
        }
    }

    public float Stamina(FFPlayer player)
    {
        try
        {
            return player != null ? player.Stamina : 0f;
        }
        catch
        {
            return 0f;
        }
    }
}
