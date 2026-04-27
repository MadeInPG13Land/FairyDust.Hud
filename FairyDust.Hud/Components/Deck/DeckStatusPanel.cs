using Il2Cppmadeinfairyland.forsakenfrontiers.actor.player.datadeck;
using Il2CppTMPro;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FairyDust.Hud.Components.Deck;

/// <summary>Compact Data Deck-style board with labeled horizontal status rows.</summary>
internal static class DeckStatusPanel
{
    public const int RowCount = 4;
    public const float StrokePx = 2f;
    public const float PadPx = 7f;
    public const float RowHeightPx = 36f;
    public const float LabelWidthPx = 132f;
    public const float BarWidthPx = 136f;
    public const float BarHeightPx = 13f;
    public const float ValueWidthPx = 68f;
    public const float LabelHeightPx = 21f;
    public const float RowDividerInsetPx = 0f;
    public const int DefaultPipCount = 3;
    public const int StaminaPipCount = 7;
    public const int MaxPipCount = StaminaPipCount;
    public const float PipSpacingPx = 4f;

    internal const bool UsePips = false;
    private static readonly Color PanelBackground = new(0f, 0f, 0f, 0.15f);
    internal static readonly Color TrackColor = new(0.02f, 0.02f, 0.02f, 0.06f);
    private static readonly Color EmptyValueColor = new(1f, 1f, 1f, 0f);
    private static Sprite whiteUnitSprite;

    public static Vector2 PreferredOuterSize =>
        OuterSizeForRows(RowCount);

    public static Vector2 OuterSizeForRows(int visibleRowCount) =>
        new(
            StrokePx * 2f + PadPx * 2f + LabelWidthPx + BarWidthPx + ValueWidthPx,
            StrokePx * 2f + PadPx * 2f + RowHeightPx * Mathf.Clamp(visibleRowCount, 1, RowCount));

    public static DeckStatusPanelHandle Build(RectTransform slot, FFDataDeck deck, string rootName)
    {
        var tintedImages = new List<Image>();
        var tintedTexts = new List<TextMeshProUGUI>();
        var dividers = new List<Image>();
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

        // Temporarily hidden while evaluating a borderless status board treatment.
        // AddLine(root.transform, deck, tintedImages, "BorderTop",
        //     new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, StrokePx), Vector2.zero);
        // AddLine(root.transform, deck, tintedImages, "BorderBottom",
        //     new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, StrokePx), Vector2.zero);
        // AddLine(root.transform, deck, tintedImages, "BorderLeft",
        //     new Vector2(0f, 0f), new Vector2(0f, 1f), new Vector2(0f, 0.5f), new Vector2(StrokePx, 0f), Vector2.zero);
        // AddLine(root.transform, deck, tintedImages, "BorderRight",
        //     new Vector2(1f, 0f), new Vector2(1f, 1f), new Vector2(1f, 0.5f), new Vector2(StrokePx, 0f), Vector2.zero);

        var content = CreateChild("Content", root.transform);
        var contentRt = content.AddComponent<RectTransform>();
        contentRt.anchorMin = Vector2.zero;
        contentRt.anchorMax = Vector2.one;
        contentRt.offsetMin = new Vector2(StrokePx + PadPx, StrokePx + PadPx);
        contentRt.offsetMax = new Vector2(-(StrokePx + PadPx), -(StrokePx + PadPx));

        var rows = new DeckStatusRow[RowCount];
        for (int i = 0; i < RowCount; i++)
        {
            rows[i] = BuildRow(contentRt, deck, tintedImages, tintedTexts, i);
        }

        for (int i = 1; i < RowCount; i++)
        {
            float y = -RowHeightPx * i;
            // Temporarily hidden while evaluating a borderless status board treatment.
            // Image divider = AddLine(content.transform, deck, tintedImages, "Divider" + i,
            //     new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0.5f, 1f),
            //     new Vector2(0f, StrokePx), new Vector2(0f, y + RowDividerInsetPx));
            // dividers.Add(divider);
        }

        return new DeckStatusPanelHandle(root, rows, background, dividers.ToArray(), tintedImages.ToArray(), tintedTexts.ToArray());
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
        track.gameObject.SetActive(!UsePips);

        var pips = new Image[MaxPipCount];
        for (int i = 0; i < pips.Length; i++)
        {
            Image pip = CreateImage("Pip" + i, barGo.transform, TrackColor);
            var pipRt = pip.rectTransform;
            pipRt.anchorMin = new Vector2(0f, 0.5f);
            pipRt.anchorMax = new Vector2(0f, 0.5f);
            pipRt.pivot = new Vector2(0f, 0.5f);
            pipRt.anchoredPosition = new Vector2(i * (BarHeightPx + PipSpacingPx), 0f);
            pipRt.sizeDelta = new Vector2(BarHeightPx, BarHeightPx);
            pip.gameObject.SetActive(UsePips);
            pips[i] = pip;
        }

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

        return new DeckStatusRow(row, label, value, track, lowWarning, fill, fillRt, pips);
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

    private static Image AddLine(
        Transform parent,
        FFDataDeck deck,
        List<Image> tintedImages,
        string name,
        Vector2 anchorMin,
        Vector2 anchorMax,
        Vector2 pivot,
        Vector2 sizeDelta,
        Vector2 anchoredPosition)
    {
        var image = CreateImage(name, parent, Color.white);
        var rt = image.rectTransform;
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.pivot = pivot;
        rt.anchoredPosition = anchoredPosition;
        rt.sizeDelta = sizeDelta;
        if (deck != null)
        {
            DeckTint.BindImage(image, deck, false);
        }

        tintedImages.Add(image);
        return image;
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

        var tex = new Texture2D(1, 1, TextureFormat.RGBA32, false);
        tex.SetPixel(0, 0, Color.white);
        tex.Apply(false, true);
        whiteUnitSprite = Sprite.Create(tex, new Rect(0f, 0f, 1f, 1f), new Vector2(0.5f, 0.5f), 100f);
        return whiteUnitSprite;
    }
}

internal readonly struct DeckStatusPanelHandle
{
    public DeckStatusPanelHandle(
        GameObject root,
        DeckStatusRow[] rows,
        Image background,
        Image[] dividers,
        Image[] tintedImages,
        TextMeshProUGUI[] tintedTexts)
    {
        Root = root;
        Rows = rows;
        Background = background;
        Dividers = dividers;
        TintedImages = tintedImages;
        TintedTexts = tintedTexts;
    }

    public GameObject Root { get; }
    public DeckStatusRow[] Rows { get; }
    private Image Background { get; }
    private Image[] Dividers { get; }
    private Image[] TintedImages { get; }
    private TextMeshProUGUI[] TintedTexts { get; }
    public bool IsValid => Root != null && Rows != null && Rows.Length == DeckStatusPanel.RowCount;

    public void SetVisible(bool visible)
    {
        try
        {
            if (Root != null)
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
                Background.enabled = alpha > 0.001f;
                Color color = Background.color;
                color.a = Mathf.Clamp01(alpha);
                Background.color = color;
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
            Rows[index].SetVisible(visible);
        }
        catch
        {
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

        if (Dividers != null)
        {
            for (int i = 0; i < Dividers.Length; i++)
            {
                Image divider = Dividers[i];
                if (divider == null)
                {
                    continue;
                }

                bool visible = i < visibleIndex - 1;
                try
                {
                    divider.gameObject.SetActive(visible);
                    if (visible)
                    {
                        divider.rectTransform.anchoredPosition = new Vector2(0f, -DeckStatusPanel.RowHeightPx * (i + 1) + DeckStatusPanel.RowDividerInsetPx);
                    }
                }
                catch
                {
                }
            }
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
                        text.alignment = TextAlignmentOptions.MidlineLeft;
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
                        image.color = flavor;
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
    private readonly Image[] pips;
    private readonly RectTransform rootRect;

    public DeckStatusRow(
        GameObject root,
        TextMeshProUGUI label,
        TextMeshProUGUI value,
        Image track,
        Image lowWarning,
        Image fill,
        RectTransform fillRect,
        Image[] pips)
    {
        this.root = root;
        rootRect = root != null ? root.GetComponent<RectTransform>() : null;
        this.label = label;
        this.value = value;
        this.track = track;
        this.lowWarning = lowWarning;
        this.fill = fill;
        this.fillRect = fillRect;
        this.pips = pips;
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
            if (root != null)
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
            root.SetActive(true);
        }

        if (label != null)
        {
            label.text = labelText ?? string.Empty;
        }

        if (track != null)
        {
            track.color = DeckStatusPanel.TrackColor;
            track.gameObject.SetActive(!DeckStatusPanel.UsePips);
        }

        if (lowWarning != null)
        {
            lowWarning.gameObject.SetActive(false);
        }

        if (fill != null)
        {
            Color baseColor = label != null ? label.color : Color.white;
            if (showLowWarning)
            {
                float wave = (Mathf.Sin(Time.unscaledTime * 4.5f) + 1f) * 0.5f;
                Color pulseColor = Color.Lerp(baseColor, Color.black, 0.65f);
                pulseColor.a = Mathf.Clamp01(baseColor.a * 0.28f);
                fill.color = Color.Lerp(baseColor, pulseColor, Mathf.Lerp(0.15f, 0.9f, wave));
            }
            else
            {
                fill.color = baseColor;
            }

            fill.gameObject.SetActive(!DeckStatusPanel.UsePips);
        }

        if (fillRect != null)
        {
            float width = DeckStatusPanel.BarWidthPx * Mathf.Clamp01(ratio);
            fillRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width < 0.5f ? 0f : width);
        }

        UpdatePips(labelText, ratio, showLowWarning);

        if (value != null)
        {
            bool hasValue = !string.IsNullOrEmpty(valueText);
            value.text = hasValue ? valueText : string.Empty;
            Color baseColor = label != null ? label.color : Color.white;
            value.color = hasValue ? baseColor : new Color(baseColor.r, baseColor.g, baseColor.b, 0f);
        }
    }

    public void RefreshFillFromLabel()
    {
        if (fill != null)
        {
            fill.color = label != null ? label.color : Color.white;
        }
    }

    private void UpdatePips(string labelText, float ratio, bool showLowWarning)
    {
        if (!DeckStatusPanel.UsePips || pips == null)
        {
            return;
        }

        int pipCount = IsStamina(labelText) ? DeckStatusPanel.StaminaPipCount : DeckStatusPanel.DefaultPipCount;
        int filledCount = Mathf.CeilToInt(Mathf.Clamp01(ratio) * pipCount);
        Color baseColor = label != null ? label.color : Color.white;
        Color warningColor = baseColor;
        if (showLowWarning)
        {
            float wave = (Mathf.Sin(Time.unscaledTime * 4.5f) + 1f) * 0.5f;
            Color pulseColor = Color.Lerp(baseColor, Color.black, 0.65f);
            pulseColor.a = Mathf.Clamp01(baseColor.a * 0.28f);
            warningColor = Color.Lerp(baseColor, pulseColor, Mathf.Lerp(0.15f, 0.9f, wave));
        }

        for (int i = 0; i < pips.Length; i++)
        {
            Image pip = pips[i];
            if (pip == null)
            {
                continue;
            }

            bool visible = i < pipCount;
            pip.gameObject.SetActive(visible);
            if (visible)
            {
                bool isLastFilledPip = showLowWarning && filledCount > 0 && i == filledCount - 1;
                pip.color = i < filledCount
                    ? isLastFilledPip
                        ? warningColor
                        : baseColor
                    : DeckStatusPanel.TrackColor;
            }
        }
    }

    private static bool IsStamina(string labelText) =>
        string.Equals(labelText, "STAMINA", StringComparison.OrdinalIgnoreCase);
}
