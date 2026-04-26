using UnityEngine;
using UnityEngine.UI;

namespace FairyDust.Hud.Components;

/// <summary>uGUI: track + left fill. <see cref="Full"/> adds the dark frame; <see cref="Slim"/> is track/fill only inside a <see cref="LineFrame"/> box.</summary>
internal static class FillBar
{
    public const float DefaultWidth = 200f;
    public const float DefaultHeight = 18f;
    public const float Pad = 2f;

    private static readonly Color Outer = new(0.15f, 0.15f, 0.15f, 0.92f);
    private static readonly Color Track = new(0.05f, 0.05f, 0.05f, 0.95f);
    private static readonly Color FillC = Color.white;

    public static Color DefaultFill => FillC;
    public static Vector2 DefaultSize => new(DefaultWidth, DefaultHeight);

    private static Sprite _white;

    public static FillBarHandle Slim(Transform parent)
    {
        float p = Pad;
        var trackGo = Child("Track", parent);
        var trackRt = trackGo.AddComponent<RectTransform>();
        trackRt.anchorMin = Vector2.zero;
        trackRt.anchorMax = Vector2.one;
        trackRt.offsetMin = new Vector2(p, p);
        trackRt.offsetMax = new Vector2(-p, -p);
        var trackImg = trackGo.AddComponent<Image>();
        trackImg.sprite = White();
        trackImg.type = Image.Type.Simple;
        trackImg.color = Track;
        trackImg.raycastTarget = false;

        // Fill width = track width at 100%: both are inset the same, so no dark “cap” at the end of the track.
        float prW = parent is RectTransform parenRt ? parenRt.sizeDelta.x : DefaultWidth;
        if (prW < 1f)
        {
            prW = DefaultWidth;
        }

        float w = prW - 2f * p;
        if (w < 1f)
        {
            w = DefaultWidth - 2f * p;
        }

        var fillGo = Child("Fill", trackRt);
        var fillRt = fillGo.AddComponent<RectTransform>();
        fillRt.anchorMin = new Vector2(0f, 0f);
        fillRt.anchorMax = new Vector2(0f, 1f);
        fillRt.pivot = new Vector2(0f, 0.5f);
        fillRt.anchoredPosition = Vector2.zero;
        fillRt.sizeDelta = Vector2.zero;
        fillRt.offsetMin = Vector2.zero;
        fillRt.offsetMax = Vector2.zero;
        var fillImg = fillGo.AddComponent<Image>();
        fillImg.sprite = White();
        fillImg.type = Image.Type.Simple;
        fillImg.color = FillC;
        fillImg.raycastTarget = false;

        return new FillBarHandle(trackGo, fillRt, fillImg, w);
    }

    public static FillBarHandle Full(Transform parent)
    {
        var shell = Child("FillBar", parent);
        var shellRt = shell.AddComponent<RectTransform>();
        shellRt.anchorMin = Vector2.zero;
        shellRt.anchorMax = Vector2.one;
        shellRt.offsetMin = Vector2.zero;
        shellRt.offsetMax = Vector2.zero;
        var shellImg = shell.AddComponent<Image>();
        shellImg.sprite = White();
        shellImg.type = Image.Type.Simple;
        shellImg.color = Outer;
        shellImg.raycastTarget = false;

        var inner = Child("Inner", shell.transform);
        var innerRt = inner.AddComponent<RectTransform>();
        float p = Pad;
        innerRt.anchorMin = Vector2.zero;
        innerRt.anchorMax = Vector2.one;
        innerRt.offsetMin = new Vector2(p, p);
        innerRt.offsetMax = new Vector2(-p, -p);

        var trackGo = Child("Track", inner.transform);
        var trackRt = trackGo.AddComponent<RectTransform>();
        trackRt.anchorMin = Vector2.zero;
        trackRt.anchorMax = Vector2.one;
        trackRt.offsetMin = Vector2.zero;
        trackRt.offsetMax = Vector2.zero;
        var trackImg = trackGo.AddComponent<Image>();
        trackImg.sprite = White();
        trackImg.type = Image.Type.Simple;
        trackImg.color = Track;
        trackImg.raycastTarget = false;

        float innerW = DefaultWidth - 2f * p;

        var fillGo = Child("Fill", trackRt.transform);
        var fillRt = fillGo.AddComponent<RectTransform>();
        fillRt.anchorMin = new Vector2(0f, 0f);
        fillRt.anchorMax = new Vector2(0f, 1f);
        fillRt.pivot = new Vector2(0f, 0.5f);
        fillRt.anchoredPosition = Vector2.zero;
        fillRt.sizeDelta = new Vector2(0f, 0f);
        fillRt.offsetMin = Vector2.zero;
        fillRt.offsetMax = Vector2.zero;
        var fillImg = fillGo.AddComponent<Image>();
        fillImg.sprite = White();
        fillImg.type = Image.Type.Simple;
        fillImg.color = FillC;
        fillImg.raycastTarget = false;

        return new FillBarHandle(shell, fillRt, fillImg, innerW);
    }

    private static GameObject Child(string name, Transform t)
    {
        var go = new GameObject(name);
        go.transform.SetParent(t, false);
        return go;
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

internal readonly struct FillBarHandle
{
    public FillBarHandle(GameObject root, RectTransform fillRect, Image fillImage, float innerW)
    {
        Root = root;
        FillRect = fillRect;
        Fill = fillImage;
        InnerW = innerW;
    }

    public GameObject Root { get; }
    public RectTransform FillRect { get; }
    public Image Fill { get; }
    public float InnerW { get; }

    public void SetRatio(float t)
    {
        if (FillRect == null)
        {
            return;
        }

        float w = InnerW * Mathf.Clamp01(t);
        if (w < 0.5f)
        {
            w = 0f;
        }

        FillRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, w);
    }
}
