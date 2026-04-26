using Il2Cppmadeinfairyland.forsakenfrontiers.actor.player.datadeck;
using UnityEngine;
using UnityEngine.UI;

namespace FairyDust.Hud.Modules.Stamina;

/// <summary>Resolves Data Deck UI sprites (blood icon, xp-style bar fill) from the live deck hierarchy.</summary>
internal static class StaminaHudBleedIcon
{
    private static readonly string[] PreferredBleedImageNames =
    {
        "Icon_Bleeding",
        "bleeding_fill",
    };

    private static FFDataDeck cachedDeck;
    private static Sprite cachedBleedSprite;

    private static FFDataDeck cachedFillDeck;
    private static Sprite cachedFillSprite;

    public static bool TryGetBleedSprite(FFDataDeck deck, out Sprite sprite)
    {
        sprite = null;
        if (deck == null)
        {
            cachedDeck = null;
            cachedBleedSprite = null;
            return false;
        }

        if (cachedDeck == deck && cachedBleedSprite != null)
        {
            sprite = cachedBleedSprite;
            return true;
        }

        cachedDeck = deck;
        cachedBleedSprite = TryFindBleedSprite(deck);
        sprite = cachedBleedSprite;
        return sprite != null;
    }

    public static bool TryGetXpFillSprite(FFDataDeck deck, out Sprite sprite)
    {
        sprite = null;
        if (deck == null)
        {
            cachedFillDeck = null;
            cachedFillSprite = null;
            return false;
        }

        if (cachedFillDeck == deck && cachedFillSprite != null)
        {
            sprite = cachedFillSprite;
            return true;
        }

        cachedFillDeck = deck;
        cachedFillSprite = TryFindNamedImageSprite(deck, "xp fill");
        sprite = cachedFillSprite;
        return sprite != null;
    }

    private static Sprite TryFindBleedSprite(FFDataDeck deck)
    {
        Sprite fromInventory = TryFindBleedUnderRoot(
            deck.inventoryUIGroup != null ? deck.inventoryUIGroup.transform : null);
        if (fromInventory != null)
        {
            return fromInventory;
        }

        return TryFindBleedUnderRoot(deck.transform);
    }

    private static Sprite TryFindBleedUnderRoot(Transform root)
    {
        if (root == null)
        {
            return null;
        }

        Image[] images = root.GetComponentsInChildren<Image>(true);
        if (images == null)
        {
            return null;
        }

        for (int p = 0; p < PreferredBleedImageNames.Length; p++)
        {
            string want = PreferredBleedImageNames[p];
            for (int i = 0; i < images.Length; i++)
            {
                Image img = images[i];
                if (img == null)
                {
                    continue;
                }

                string n = img.gameObject.name ?? string.Empty;
                if (!string.Equals(n, want, StringComparison.Ordinal))
                {
                    continue;
                }

                Sprite sp = img.sprite;
                if (sp != null)
                {
                    return sp;
                }
            }
        }

        for (int i = 0; i < images.Length; i++)
        {
            Image img = images[i];
            if (img == null)
            {
                continue;
            }

            string n = img.gameObject.name ?? string.Empty;
            if (n.IndexOf("bleed", StringComparison.OrdinalIgnoreCase) < 0)
            {
                continue;
            }

            Sprite sp = img.sprite;
            if (sp != null)
            {
                return sp;
            }
        }

        return null;
    }

    private static Sprite TryFindNamedImageSprite(FFDataDeck deck, string objectName)
    {
        Transform[] roots =
        {
            deck.inventoryUIGroup != null ? deck.inventoryUIGroup.transform : null,
            deck.transform,
        };

        for (int r = 0; r < roots.Length; r++)
        {
            Transform root = roots[r];
            if (root == null)
            {
                continue;
            }

            Image[] images = root.GetComponentsInChildren<Image>(true);
            if (images == null)
            {
                continue;
            }

            for (int i = 0; i < images.Length; i++)
            {
                Image img = images[i];
                if (img == null)
                {
                    continue;
                }

                string n = img.gameObject.name ?? string.Empty;
                if (!string.Equals(n, objectName, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (img.sprite != null)
                {
                    return img.sprite;
                }
            }
        }

        return null;
    }
}
