using FairyDust.Hud.Modules.Status.Entities;
using Il2Cppmadeinfairyland.forsakenfrontiers.actor.player;
using Il2Cppmadeinfairyland.forsakenfrontiers.actor.player.datadeck;

namespace FairyDust.Hud.Modules.Status.Services;

internal sealed class StatusVisibilityService
{
    private readonly PlayerStatusReadService reads;

    public StatusVisibilityService(PlayerStatusReadService reads)
    {
        this.reads = reads;
    }

    public bool ShouldShowBoard(FFPlayer player, FFDataDeck deck, IReadOnlyList<IStatusProvider> providers)
    {
        if (!reads.IsUsable(player) || reads.IsDataDeckOpen(deck))
        {
            return false;
        }

        for (int i = 0; i < providers.Count; i++)
        {
            if (providers[i].IsEnabled)
            {
                return true;
            }
        }

        return false;
    }
}
