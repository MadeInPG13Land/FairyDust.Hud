using Il2Cppmadeinfairyland.forsakenfrontiers.actor.player.datadeck;
using Il2CppTMPro;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FairyDust.Hud.Components.Deck;

/// <summary>Compact Data Deck-style board with labeled horizontal status rows.</summary>
internal static class DeckStatusPanel
{
    public const int DefaultRowCapacity = 4;
    public const float StrokePx = 2f;
    public const float PadPx = 7f;
    public const float RowHeightPx = 36f;
    public const float LabelWidthPx = 132f;
    public const float BarWidthPx = 136f;
    public const float BarHeightPx = 13f;
    public const float ValueWidthPx = 68f;
    public const float LabelHeightPx = 21f;

    private static readonly Color PanelBackground = new(0f, 0f, 0f, 0.15f);
    internal static readonly Color TrackColor = new(0.02f, 0.02f, 0.02f, 0.06f);
    private static readonly Color EmptyValueColor = new(1f, 1f, 1f, 0f);
    private static Sprite whiteUnitSprite;
    private static Texture2D whiteUnitTexture;

    public static Vector2 PreferredOuterSize =>
        OuterSizeForRows(DefaultRowCapacity);

    public static Vector2 OuterSizeForRows(int visibleRowCount) =>
        new(
            StrokePx * 2f + PadPx * 2f + LabelWidthPx + BarWidthPx + ValueWidthPx,
            StrokePx * 2f + PadPx * 2f + RowHeightPx * Mathf.Max(1, visibleRowCount));

    public static DeckStatusPanelHandle Build(RectTransform slot, FFDataDeck deck, string rootName, int rowCapacity)
    {
        rowCapacity = Mathf.Max(1, rowCapacity);
        var tintedImages = new List<Image>();
        var tintedTexts = new List<TextMeshProUGUI>();
        var root = CreateChild(rootName, slot);
        var rootRt = root.AddComponent<RectTransform>();
        rootRt.anchorMin = Vector2.zero;
        rootRt.anchorMax = Vector2.one;
        rootRt.offsetMin = Vector2.zero;
        rootRt.offsetMax = Vector2.zero;

        var background = CreateImage("Background", root.transform, PanelBackground);
        var backgroundRt = background.rectTransform;
        backgroundRt.anchorMin = Vector2.zero;
        backgroundRt.anchorMax = Vector2.one;
        backgroundRt.offsetMin = new Vector2(StrokePx, StrokePx);
        backgroundRt.offsetMax = new Vector2(-StrokePx, -StrokePx);

        var content = CreateChild("Content", root.transform);
        var contentRt = content.AddComponent<RectTransform>();
        contentRt.anchorMin = Vector2.zero;
        contentRt.anchorMax = Vector2.one;
        contentRt.offsetMin = new Vector2(StrokePx + PadPx, StrokePx + PadPx);
        contentRt.offsetMax = new Vector2(-(StrokePx + PadPx), -(StrokePx + PadPx));

        var rows = new DeckStatusRow[rowCapacity];
        for (int i = 0; i < rowCapacity; i++)
        {
            rows[i] = BuildRow(contentRt, deck, tintedImages, tintedTexts, i);
        }

        return new DeckStatusPanelHandle(root, rows, background, tintedImages.ToArray(), tintedTexts.ToArray());
    }

    private static DeckStatusRow BuildRow(
        RectTransform parent,
        FFDataDeck deck,
        List<Image> tintedImages,
        List<TextMeshProUGUI> tintedTexts,
        int index)
    {
        var row = CreateChild("Row" + index, parent);
        var rowRt = row.AddComponent<RectTransform>();
        rowRt.anchorMin = new Vector2(0f, 1f);
        rowRt.anchorMax = new Vector2(1f, 1f);
        rowRt.pivot = new Vector2(0f, 1f);
        rowRt.anchoredPosition = new Vector2(0f, -RowHeightPx * index);
        rowRt.sizeDelta = new Vector2(0f, RowHeightPx);

        var labelGo = CreateChild("Label", row.transform);
        var labelRt = labelGo.AddComponent<RectTransform>();
        labelRt.anchorMin = new Vector2(0f, 0.5f);
        labelRt.anchorMax = new Vector2(0f, 0.5f);
        labelRt.pivot = new Vector2(0f, 0.5f);
        labelRt.anchoredPosition = Vector2.zero;
        labelRt.sizeDelta = new Vector2(LabelWidthPx, LabelHeightPx);
        var label = labelGo.AddComponent<TextMeshProUGUI>();
        SetupText(label, deck, LabelHeightPx, false);
        tintedTexts.Add(label);

        var barGo = CreateChild("Bar", row.transform);
        var barRt = barGo.AddComponent<RectTransform>();
        barRt.anchorMin = new Vector2(0f, 0.5f);
        barRt.anchorMax = new Vector2(0f, 0.5f);
        barRt.pivot = new Vector2(0f, 0.5f);
        barRt.anchoredPosition = new Vector2(LabelWidthPx, 0f);
        barRt.sizeDelta = new Vector2(BarWidthPx, BarHeightPx);

        var track = CreateImage("Track", barGo.transform, TrackColor);
        var trackRt = track.rectTransform;
        trackRt.anchorMin = Vector2.zero;
        trackRt.anchorMax = Vector2.one;
        trackRt.offsetMin = Vector2.zero;
        trackRt.offsetMax = Vector2.zero;

        var lowWarning = CreateImage("LowWarning", track.transform, Color.white);
        var lowWarningRt = lowWarning.rectTransform;
        lowWarningRt.anchorMin = Vector2.zero;
        lowWarningRt.anchorMax = Vector2.one;
        lowWarningRt.offsetMin = new Vector2(-4f, -3f);
        lowWarningRt.offsetMax = new Vector2(4f, 3f);
        lowWarning.gameObject.SetActive(false);

        var fill = CreateImage("Fill", track.transform, Color.white);
        var fillRt = fill.rectTransform;
        fillRt.anchorMin = new Vector2(0f, 0f);
        fillRt.anchorMax = new Vector2(0f, 1f);
        fillRt.pivot = new Vector2(0f, 0.5f);
        fillRt.anchoredPosition = Vector2.zero;
        fillRt.sizeDelta = Vector2.zero;
        if (deck != null)
        {
            DeckTint.BindImage(fill, deck, false);
        }
        tintedImages.Add(fill);

        var valueGo = CreateChild("Value", row.transform);
        var valueRt = valueGo.AddComponent<RectTransform>();
        valueRt.anchorMin = new Vector2(0f, 0.5f);
        valueRt.anchorMax = new Vector2(0f, 0.5f);
        valueRt.pivot = new Vector2(1f, 0.5f);
        valueRt.anchoredPosition = new Vector2(LabelWidthPx + BarWidthPx + ValueWidthPx, 0f);
        valueRt.sizeDelta = new Vector2(ValueWidthPx, LabelHeightPx);
        var value = valueGo.AddComponent<TextMeshProUGUI>();
        SetupText(value, deck, LabelHeightPx, true);
        value.color = EmptyValueColor;
        tintedTexts.Add(value);

        return new DeckStatusRow(row, label, value, track, lowWarning, fill, fillRt);
    }

    private static void SetupText(TextMeshProUGUI text, FFDataDeck deck, float targetHeight, bool rightAligned, bool bindTint = true)
    {
        text.raycastTarget = false;
        text.enableWordWrapping = false;
        text.overflowMode = TextOverflowModes.Overflow;
        text.margin = Vector4.zero;
        text.color = Color.white;
        DeckTextStyle.Apply(text, deck, targetHeight);
        text.alignment = rightAligned
            ? TextAlignmentOptions.MidlineRight
            : TextAlignmentOptions.MidlineLeft;
        if (bindTint && deck != null)
        {
            DeckTint.BindText(text, deck, false);
        }
    }

    private static Image CreateImage(string name, Transform parent, Color color)
    {
        var go = CreateChild(name, parent);
        var image = go.AddComponent<Image>();
        image.sprite = White();
        image.type = Image.Type.Simple;
        image.color = color;
        image.raycastTarget = false;
        return image;
    }

    private static GameObject CreateChild(string name, Transform parent)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        return go;
    }

    private static Sprite White()
    {
        if (whiteUnitSprite != null)
        {
            return whiteUnitSprite;
        }

        whiteUnitTexture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
        whiteUnitTexture.SetPixel(0, 0, Color.white);
        whiteUnitTexture.Apply(false, true);
        whiteUnitSprite = Sprite.Create(whiteUnitTexture, new Rect(0f, 0f, 1f, 1f), new Vector2(0.5f, 0.5f), 100f);
        return whiteUnitSprite;
    }

    public static void ReleaseSharedAssets()
    {
        try
        {
            if (whiteUnitSprite != null)
            {
                UnityEngine.Object.Destroy(whiteUnitSprite);
                whiteUnitSprite = null;
            }

            if (whiteUnitTexture != null)
            {
                UnityEngine.Object.Destroy(whiteUnitTexture);
                whiteUnitTexture = null;
            }
        }
        catch
        {
        }
    }
}

internal readonly struct DeckStatusPanelHandle
{
    public DeckStatusPanelHandle(
        GameObject root,
        DeckStatusRow[] rows,
        Image background,
        Image[] tintedImages,
        TextMeshProUGUI[] tintedTexts)
    {
        Root = root;
        Rows = rows;
        Background = background;
        TintedImages = tintedImages;
        TintedTexts = tintedTexts;
    }

    public GameObject Root { get; }
    public DeckStatusRow[] Rows { get; }
    private Image Background { get; }
    private Image[] TintedImages { get; }
    private TextMeshProUGUI[] TintedTexts { get; }
    public bool IsValid => Root != null && Rows != null && Rows.Length > 0;

    public void SetVisible(bool visible)
    {
        try
        {
            if (Root != null && Root.activeSelf != visible)
            {
                Root.SetActive(visible);
            }
        }
        catch
        {
        }
    }

    public void SetRow(int index, string label, float ratio, string valueText = null, bool lowWarning = false)
    {
        if (!IsValid || index < 0 || index >= Rows.Length)
        {
            return;
        }

        try
        {
            Rows[index].Set(label, ratio, valueText, lowWarning);
        }
        catch
        {
        }
    }

    public void SetBackgroundAlpha(float alpha)
    {
        try
        {
            if (Background != null)
            {
                bool enabled = alpha > 0.001f;
                if (Background.enabled != enabled)
                {
                    Background.enabled = enabled;
                }

                Color color = Background.color;
                color.a = Mathf.Clamp01(alpha);
                if (Background.color != color)
                {
                    Background.color = color;
                }
            }
        }
        catch
        {
        }
    }

    public void SetRowVisible(int index, bool visible)
    {
        if (!IsValid || index < 0 || index >= Rows.Length)
        {
            return;
        }

        try
        {
            if (Rows[index].IsVisible != visible)
            {
                Rows[index].SetVisible(visible);
            }
        }
        catch
        {
        }
    }

    public void ClearRows()
    {
        if (!IsValid)
        {
            return;
        }

        for (int i = 0; i < Rows.Length; i++)
        {
            try
            {
                if (Rows[i].IsVisible)
                {
                    Rows[i].SetVisible(false);
                }
            }
            catch
            {
            }
        }
    }

    public int ApplyVisibleLayout()
    {
        if (!IsValid)
        {
            return 0;
        }

        int visibleIndex = 0;
        for (int i = 0; i < Rows.Length; i++)
        {
            bool isVisible;
            try
            {
                isVisible = Rows[i].IsVisible;
            }
            catch
            {
                continue;
            }

            if (!isVisible)
            {
                continue;
            }

            try
            {
                Rows[i].SetVisualIndex(visibleIndex);
            }
            catch
            {
            }
            visibleIndex++;
        }

        return Mathf.Max(1, visibleIndex);
    }

    public bool RefreshFlavor(FFDataDeck deck, bool refreshTextStyle)
    {
        if (!IsValid || !DeckTextStyle.TryResolveDeckColor(deck, out Color flavor))
        {
            return false;
        }

        if (TintedTexts != null)
        {
            for (int i = 0; i < TintedTexts.Length; i++)
            {
                TextMeshProUGUI text = TintedTexts[i];
                if (text == null)
                {
                    continue;
                }

                try
                {
                    if (refreshTextStyle)
                    {
                        DeckTextStyle.Apply(text, deck, DeckStatusPanel.LabelHeightPx);
                        text.alignment = string.Equals(text.gameObject.name, "Value", StringComparison.Ordinal)
                            ? TextAlignmentOptions.MidlineRight
                            : TextAlignmentOptions.MidlineLeft;
                    }

                    text.color = flavor;
                }
                catch
                {
                }
            }
        }

        if (TintedImages != null)
        {
            for (int i = 0; i < TintedImages.Length; i++)
            {
                Image image = TintedImages[i];
                try
                {
                    if (image != null)
                    {
                        if (image.color != flavor)
                        {
                            image.color = flavor;
                        }
                    }
                }
                catch
                {
                }
            }
        }

        return true;
    }

    public void Destroy()
    {
        try
        {
            if (Root != null)
            {
                UnityEngine.Object.Destroy(Root);
            }
        }
        catch
        {
        }
    }
}

internal readonly struct DeckStatusRow
{
    private readonly GameObject root;
    private readonly TextMeshProUGUI label;
    private readonly TextMeshProUGUI value;
    private readonly Image track;
    private readonly Image lowWarning;
    private readonly Image fill;
    private readonly RectTransform fillRect;
    private readonly RectTransform rootRect;

    public DeckStatusRow(
        GameObject root,
        TextMeshProUGUI label,
        TextMeshProUGUI value,
        Image track,
        Image lowWarning,
        Image fill,
        RectTransform fillRect)
    {
        this.root = root;
        rootRect = root != null ? root.GetComponent<RectTransform>() : null;
        this.label = label;
        this.value = value;
        this.track = track;
        this.lowWarning = lowWarning;
        this.fill = fill;
        this.fillRect = fillRect;
    }

    public bool IsVisible
    {
        get
        {
            try
            {
                return root != null && root.activeSelf;
            }
            catch
            {
                return false;
            }
        }
    }

    public void SetVisible(bool visible)
    {
        try
        {
            if (root != null && root.activeSelf != visible)
            {
                root.SetActive(visible);
            }
        }
        catch
        {
        }
    }

    public void SetVisualIndex(int index)
    {
        try
        {
            if (rootRect != null)
            {
                rootRect.anchoredPosition = new Vector2(0f, -DeckStatusPanel.RowHeightPx * index);
            }
        }
        catch
        {
        }
    }

    public void Set(string labelText, float ratio, string valueText, bool showLowWarning)
    {
        if (root != null)
        {
            if (!root.activeSelf)
            {
                root.SetActive(true);
            }
        }

        if (label != null)
        {
            string nextLabel = labelText ?? string.Empty;
            if (!string.Equals(label.text, nextLabel, StringComparison.Ordinal))
            {
                label.text = nextLabel;
            }
        }

        if (track != null)
        {
            if (track.color != DeckStatusPanel.TrackColor)
            {
                track.color = DeckStatusPanel.TrackColor;
            }

            if (!track.gameObject.activeSelf)
            {
                track.gameObject.SetActive(true);
            }
        }

        if (lowWarning != null)
        {
            if (lowWarning.gameObject.activeSelf)
            {
                lowWarning.gameObject.SetActive(false);
            }
        }

        if (fill != null)
        {
            Color baseColor = label != null ? label.color : Color.white;
            if (showLowWarning)
            {
                float wave = (Mathf.Sin(Time.unscaledTime * 4.5f) + 1f) * 0.5f;
                Color pulseColor = Color.Lerp(baseColor, Color.black, 0.65f);
                pulseColor.a = Mathf.Clamp01(baseColor.a * 0.28f);
                Color nextFillColor = Color.Lerp(baseColor, pulseColor, Mathf.Lerp(0.15f, 0.9f, wave));
                if (fill.color != nextFillColor)
                {
                    fill.color = nextFillColor;
                }
            }
            else
            {
                if (fill.color != baseColor)
                {
                    fill.color = baseColor;
                }
            }

            if (!fill.gameObject.activeSelf)
            {
                fill.gameObject.SetActive(true);
            }
        }

        if (fillRect != null)
        {
            float width = DeckStatusPanel.BarWidthPx * Mathf.Clamp01(ratio);
            fillRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width < 0.5f ? 0f : width);
        }

        if (value != null)
        {
            bool hasValue = !string.IsNullOrEmpty(valueText);
            string nextValue = hasValue ? valueText : string.Empty;
            if (!string.Equals(value.text, nextValue, StringComparison.Ordinal))
            {
                value.text = nextValue;
            }

            Color baseColor = label != null ? label.color : Color.white;
            Color nextValueColor = hasValue ? baseColor : new Color(baseColor.r, baseColor.g, baseColor.b, 0f);
            if (value.color != nextValueColor)
            {
                value.color = nextValueColor;
            }
        }
    }

}
