using FairyDust.Hud.Components.Deck;
using Il2Cppmadeinfairyland.forsakenfrontiers.actor.player.datadeck;
using UnityEngine;
using UnityEngine.UI;

namespace FairyDust.Hud.Components;

/// <summary>Thin <see cref="Image"/> frame strips (Data Deck / grid style) + optional <see cref="FFDataDeck"/> tint.</summary>
internal static class LineFrame
{
    public const float Hairline = 1f;
    private static Sprite _white;

    public static RectTransform Build(Transform parent, float px, FFDataDeck deck)
    {
        Strip(parent, "T", deck, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, px));
        Strip(parent, "B", deck, new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, px));
        Strip(parent, "L", deck, new Vector2(0f, 0f), new Vector2(0f, 1f), new Vector2(0f, 0.5f), new Vector2(px, 0f));
        Strip(parent, "R", deck, new Vector2(1f, 0f), new Vector2(1f, 1f), new Vector2(1f, 0.5f), new Vector2(px, 0f));

        var c = new GameObject("Content");
        c.transform.SetParent(parent, false);
        var rt = c.AddComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.offsetMin = new Vector2(px, px);
        rt.offsetMax = new Vector2(-px, -px);
        return rt;
    }

    public static void VLine(Transform content, float x, float px, FFDataDeck deck)
    {
        var go = new GameObject("V");
        go.transform.SetParent(content, false);
        var rt = go.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(0f, 0f);
        rt.anchorMax = new Vector2(0f, 1f);
        rt.pivot = new Vector2(0f, 0.5f);
        rt.anchoredPosition = new Vector2(x, 0f);
        rt.sizeDelta = new Vector2(px, 0f);
        var img = go.AddComponent<Image>();
        img.sprite = White();
        img.type = Image.Type.Simple;
        img.color = Color.white;
        img.raycastTarget = false;
        if (deck != null)
        {
            DeckTint.BindImage(img, deck, false);
        }
    }

    private static void Strip(
        Transform parent,
        string name,
        FFDataDeck deck,
        Vector2 a0,
        Vector2 a1,
        Vector2 pivot,
        Vector2 sizeDelta)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var rt = go.AddComponent<RectTransform>();
        rt.anchorMin = a0;
        rt.anchorMax = a1;
        rt.pivot = pivot;
        rt.anchoredPosition = Vector2.zero;
        rt.sizeDelta = sizeDelta;
        var img = go.AddComponent<Image>();
        img.sprite = White();
        img.type = Image.Type.Simple;
        img.color = Color.white;
        img.raycastTarget = false;
        if (deck != null)
        {
            DeckTint.BindImage(img, deck, false);
        }
    }

    private static Sprite White()
    {
        if (_white != null)
        {
            return _white;
        }

        var tex = new Texture2D(1, 1, TextureFormat.RGBA32, false);
        tex.SetPixel(0, 0, Color.white);
        tex.Apply(false, true);
        _white = Sprite.Create(tex, new Rect(0f, 0f, 1f, 1f), new Vector2(0.5f, 0.5f), 100f);
        return _white;
    }
}
