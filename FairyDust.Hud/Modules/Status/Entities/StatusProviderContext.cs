using Il2Cppmadeinfairyland.forsakenfrontiers.actor.player;
using Il2Cppmadeinfairyland.forsakenfrontiers.actor.player.datadeck;

namespace FairyDust.Hud.Modules.Status.Entities;

internal readonly struct StatusProviderContext
{
    public StatusProviderContext(FFPlayer player, FFDataDeck deck)
    {
        Player = player;
        Deck = deck;
    }

    public FFPlayer Player { get; }

    public FFDataDeck Deck { get; }
}
