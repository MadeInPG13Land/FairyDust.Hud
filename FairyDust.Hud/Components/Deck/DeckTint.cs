using Il2Cppmadeinfairyland.forsakenfrontiers.actor.player.datadeck;
using Il2Cppmadeinfairyland.forsakenfrontiers.ui;
using Il2CppTMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FairyDust.Hud.Components.Deck;

/// <summary>Applies Data Deck flavor colors without attaching game-owned tint components to DDOL HUD UI.</summary>
internal static class DeckTint
{
    public static FFDataDeckUIColorer BindImage(Image image, FFDataDeck deck, bool darken)
    {
        if (image == null)
        {
            return null;
        }

        Color color = DeckTextStyle.ResolveDeckColor(deck, image.color);
        image.color = darken ? Darken(color) : color;
        return null;
    }

    public static FFDataDeckUIColorer BindText(TextMeshProUGUI text, FFDataDeck deck, bool darken)
    {
        if (text == null)
        {
            return null;
        }

        Color color = DeckTextStyle.ResolveDeckColor(deck, text.color);
        text.color = darken ? Darken(color) : color;
        return null;
    }

    private static Color Darken(Color color)
    {
        return new Color(color.r * 0.6f, color.g * 0.6f, color.b * 0.6f, color.a);
    }
}
