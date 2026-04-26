using Il2Cppmadeinfairyland.forsakenfrontiers;
using Il2Cppmadeinfairyland.forsakenfrontiers.actor.player.datadeck;
using Il2Cppmadeinfairyland.forsakenfrontiers.ui;
using Il2CppTMPro;
using UnityEngine.UI;

namespace FairyDust.Hud.Components.Deck;

/// <summary>Attaches <see cref="FFDataDeckUIColorer"/> so HUD quads follow deck / flavor like built-in UI.</summary>
internal static class DeckTint
{
    public static FFDataDeckUIColorer BindImage(Image image, FFDataDeck deck, bool darken)
    {
        var c = image.gameObject.AddComponent<FFDataDeckUIColorer>();
        c._image = image;
        c.darkenImage = darken;
        c.AssignDataDeck(deck);
        c.ListenForFlavorChanges();
        return c;
    }

    public static FFDataDeckUIColorer BindText(TextMeshProUGUI text, FFDataDeck deck, bool darken)
    {
        var c = text.gameObject.AddComponent<FFDataDeckUIColorer>();
        c._text = text;
        c.darkenText = darken;
        c.AssignDataDeck(deck);
        c.ListenForFlavorChanges();
        return c;
    }

    public static void Apply(FFDataDeckUIColorer c, FFGameplayStatics.FlavorData f)
    {
        if (c == null)
        {
            return;
        }

        c.ApplyColor(f);
    }

    public static void SyncAll(FFDataDeckUIColorer first, FFDataDeckUIColorer[] rest, FFGameplayStatics.FlavorData f)
    {
        Apply(first, f);
        if (rest == null)
        {
            return;
        }

        for (int i = 0; i < rest.Length; i++)
        {
            Apply(rest[i], f);
        }
    }
}
