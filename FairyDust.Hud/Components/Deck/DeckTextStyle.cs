using FairyDust.Hud.Game.Services;
using Il2Cppmadeinfairyland.forsakenfrontiers.actor.player.datadeck;
using Il2CppTMPro;
using UnityEngine;

namespace FairyDust.Hud.Components.Deck;

/// <summary>Copies the live Data Deck TMP style onto overlay labels.</summary>
internal static class DeckTextStyle
{
    public static void Apply(TextMeshProUGUI label, FFDataDeck deck, float targetHeight)
    {
        if (label == null)
        {
            return;
        }

        TextMeshProUGUI src = TryGetStyleSource(deck);

        if (src == null)
        {
            label.fontSize = Mathf.Clamp(targetHeight, 10f, 24f);
            label.alignment = TextAlignmentOptions.TopLeft;
            return;
        }

        try
        {
            label.font = src.font;
            if (src.fontSharedMaterial != null)
            {
                label.fontSharedMaterial = src.fontSharedMaterial;
            }

            label.fontStyle = src.fontStyle;
            label.fontWeight = src.fontWeight;
            label.characterSpacing = src.characterSpacing;
            label.wordSpacing = src.wordSpacing;
        }
        catch
        {
            label.fontSize = Mathf.Clamp(targetHeight, 10f, 24f);
            label.alignment = TextAlignmentOptions.TopLeft;
            return;
        }

        float srcH = 40f;
        if (src.rectTransform != null)
        {
            srcH = Mathf.Max(1f, src.rectTransform.sizeDelta.y);
        }

        label.fontSize = Mathf.Clamp(src.fontSize * (targetHeight / srcH), 10f, 24f);
        label.alignment = TextAlignmentOptions.TopLeft;
        label.lineSpacing = 0f;
        label.paragraphSpacing = 0f;
    }

    public static Color ResolveDeckColor(FFDataDeck deck, Color fallback)
    {
        if (DataDeckFlavorService.TryReadFlavorColor(deck, out Color flavorColor))
        {
            return flavorColor;
        }

        TextMeshProUGUI src = TryGetStyleSource(deck);
        if (src == null)
        {
            return fallback;
        }

        try
        {
            return src.color;
        }
        catch
        {
            return fallback;
        }
    }

    public static bool TryResolveDeckColor(FFDataDeck deck, out Color color)
    {
        color = default;
        if (DataDeckFlavorService.TryReadFlavorColor(deck, out color))
        {
            return true;
        }

        TextMeshProUGUI src = TryGetStyleSource(deck);
        if (src == null)
        {
            return false;
        }

        try
        {
            color = src.color;
            return true;
        }
        catch
        {
            return false;
        }
    }

    private static TextMeshProUGUI TryGetStyleSource(FFDataDeck deck)
    {
        if (deck == null)
        {
            return null;
        }

        TextMeshProUGUI[] candidates;
        try
        {
            candidates = new[]
            {
                deck.gameTime,
                deck.characterName,
                deck.credits,
                deck.flavorText,
            };
        }
        catch
        {
            return null;
        }

        for (int i = 0; i < candidates.Length; i++)
        {
            TextMeshProUGUI candidate = candidates[i];
            if (!IsUsable(candidate))
            {
                continue;
            }

            return candidate;
        }

        return null;
    }

    private static bool IsUsable(TextMeshProUGUI text)
    {
        if (text == null)
        {
            return false;
        }

        try
        {
            _ = text.gameObject;
            _ = text.rectTransform;
            _ = text.fontSize;
            return true;
        }
        catch
        {
            return false;
        }
    }
}
