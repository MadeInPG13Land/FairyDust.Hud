using System;
using Il2Cppmadeinfairyland.forsakenfrontiers.actor.player.datadeck;
using Il2Cppmadeinfairyland.forsakenfrontiers.ui;
using UnityEngine;
using UnityEngine.UI;

namespace FairyDust.Hud.Components.Deck;

/// <summary>Data Deck–style row: frame, icon | divider | label + bar row.</summary>
internal static class DeckPanel
{
    public const int StrokePx = 2;
    public const int ContentPadPx = 6;
    public const int IconColumnWidth = 42;
    public const int IconImageInsetPx = 6;
    public const int RightColumnMinWidth = 188;
    public const int LabelHeightPx = 15;
    public const int BarHeightPx = 14;
    public const int LabelPadTopPx = 4;
    public const int LabelToBarGapPx = 4;
    public const int RowPadBottomPx = 4;

    private static Sprite whiteUnitSprite;

    /// <summary>Width/height of the content row inside the chrome inner rect (manual layout, no HLG).</summary>
    public static float InnerRowWidth =>
        IconColumnWidth + StrokePx + RightColumnMinWidth + 2 * ContentPadPx;

    public static float InnerRowHeight =>
        LabelPadTopPx + LabelHeightPx + LabelToBarGapPx + BarHeightPx + RowPadBottomPx;

    /// <summary>Slot <see cref="LayoutElement"/> size for a panel built with current metrics.</summary>
    public static Vector2 PreferredOuterSize
    {
        get
        {
            float inset = StrokePx + ContentPadPx;
            return new Vector2(InnerRowWidth + 2f * inset, InnerRowHeight + 2f * inset);
        }
    }

    public delegate bool TryDeckSprite(FFDataDeck deck, out Sprite sprite);

    public static DeckParts Build(
        RectTransform slot,
        FFDataDeck deck,
        string rowObjectName,
        string labelText,
        int labelFontSize,
        TryDeckSprite tryIconSprite,
        TryDeckSprite tryBarFillSprite)
    {
        var chromeGo = CreateChild("Chrome", slot);
        var chromeRt = chromeGo.AddComponent<RectTransform>();
        chromeRt.anchorMin = Vector2.zero;
        chromeRt.anchorMax = Vector2.one;
        chromeRt.offsetMin = Vector2.zero;
        chromeRt.offsetMax = Vector2.zero;

        float inset = StrokePx + ContentPadPx;
        RectTransform innerRt = AddChromeInner(chromeGo.transform, inset);
        FFDataDeckUIColorer[] borderColorers = AddChromeBorderStrips(chromeGo.transform, StrokePx, deck);

        var row = CreateChild(rowObjectName, innerRt);
        var rowRt = row.AddComponent<RectTransform>();
        rowRt.anchorMin = Vector2.zero;
        rowRt.anchorMax = Vector2.one;
        rowRt.offsetMin = Vector2.zero;
        rowRt.offsetMax = Vector2.zero;

        var iconHost = CreateChild("IconColumn", rowRt.transform);
        var iconColRt = iconHost.AddComponent<RectTransform>();
        iconColRt.anchorMin = new Vector2(0f, 0f);
        iconColRt.anchorMax = new Vector2(0f, 1f);
        iconColRt.pivot = new Vector2(0f, 0.5f);
        iconColRt.anchoredPosition = Vector2.zero;
        iconColRt.sizeDelta = new Vector2(IconColumnWidth, 0f);

        var iconGo = CreateChild("Icon", iconHost.transform);
        var iconRt = iconGo.AddComponent<RectTransform>();
        iconRt.anchorMin = Vector2.zero;
        iconRt.anchorMax = Vector2.one;
        float ip = IconImageInsetPx;
        iconRt.offsetMin = new Vector2(ip, ip);
        iconRt.offsetMax = new Vector2(-ip, -ip);
        var iconImage = iconGo.AddComponent<Image>();
        iconImage.preserveAspect = true;
        iconImage.raycastTarget = false;
        if (tryIconSprite(deck, out Sprite ic))
        {
            iconImage.sprite = ic;
            iconImage.color = Color.white;
        }
        else
        {
            iconImage.color = new Color(1f, 1f, 1f, 0f);
        }

        float divVInset = ContentPadPx;
        var divHost = CreateChild("Divider", rowRt.transform);
        var divRt = divHost.AddComponent<RectTransform>();
        divRt.anchorMin = new Vector2(0f, 0f);
        divRt.anchorMax = new Vector2(0f, 1f);
        divRt.pivot = new Vector2(0f, 0.5f);
        divRt.anchoredPosition = new Vector2(IconColumnWidth, 0f);
        divRt.sizeDelta = new Vector2(StrokePx, 0f);
        divRt.offsetMin = new Vector2(0f, divVInset);
        divRt.offsetMax = new Vector2(0f, -divVInset);
        var dividerImage = divHost.AddComponent<Image>();
        dividerImage.sprite = GetWhiteUnitSprite();
        dividerImage.color = Color.white;
        dividerImage.raycastTarget = false;
        FFDataDeckUIColorer dividerColorer = DeckTint.BindImage(dividerImage, deck, false);

        float hPad = ContentPadPx;
        var rightHost = CreateChild("Right", rowRt.transform);
        var rightRt = rightHost.AddComponent<RectTransform>();
        rightRt.anchorMin = new Vector2(0f, 0f);
        rightRt.anchorMax = new Vector2(0f, 1f);
        rightRt.pivot = new Vector2(0f, 0.5f);
        rightRt.anchoredPosition = new Vector2(IconColumnWidth + StrokePx, 0f);
        rightRt.sizeDelta = new Vector2(RightColumnMinWidth, 0f);
        rightRt.offsetMin = new Vector2(hPad, 0f);
        rightRt.offsetMax = new Vector2(-hPad, 0f);

        var labelGo = CreateChild("Label", rightHost.transform);
        var labelRt = labelGo.AddComponent<RectTransform>();
        labelRt.anchorMin = new Vector2(0f, 1f);
        labelRt.anchorMax = new Vector2(1f, 1f);
        labelRt.pivot = new Vector2(0f, 1f);
        labelRt.anchoredPosition = new Vector2(0f, -LabelPadTopPx);
        labelRt.sizeDelta = new Vector2(0f, LabelHeightPx);
        var label = labelGo.AddComponent<Text>();
        label.text = labelText;
        label.font = ResolveUiFont();
        label.fontSize = labelFontSize;
        label.fontStyle = FontStyle.Bold;
        label.color = Color.white;
        label.alignment = TextAnchor.UpperLeft;
        label.horizontalOverflow = HorizontalWrapMode.Overflow;
        label.verticalOverflow = VerticalWrapMode.Truncate;
        label.raycastTarget = false;

        var barGo = CreateChild("Bar", rightHost.transform);
        var barRt = barGo.AddComponent<RectTransform>();
        barRt.anchorMin = new Vector2(0f, 1f);
        barRt.anchorMax = new Vector2(1f, 1f);
        barRt.pivot = new Vector2(0f, 1f);
        float barTop = -(LabelPadTopPx + LabelHeightPx + LabelToBarGapPx);
        barRt.anchoredPosition = new Vector2(0f, barTop);
        barRt.sizeDelta = new Vector2(0f, BarHeightPx);
        barGo.AddComponent<RectMask2D>();

        var trackGo = CreateChild("Track", barGo.transform);
        var trackRt = trackGo.AddComponent<RectTransform>();
        trackRt.anchorMin = Vector2.zero;
        trackRt.anchorMax = Vector2.one;
        trackRt.offsetMin = Vector2.zero;
        trackRt.offsetMax = Vector2.zero;
        var trackImage = trackGo.AddComponent<Image>();
        trackImage.sprite = GetWhiteUnitSprite();
        trackImage.color = new Color(0.1f, 0.1f, 0.1f, 1f);
        trackImage.raycastTarget = false;

        var fillGo = CreateChild("Fill", trackRt.transform);
        var fillRt = fillGo.AddComponent<RectTransform>();
        fillRt.anchorMin = Vector2.zero;
        fillRt.anchorMax = Vector2.one;
        fillRt.offsetMin = Vector2.zero;
        fillRt.offsetMax = Vector2.zero;
        var fillImage = fillGo.AddComponent<Image>();
        fillImage.sprite = tryBarFillSprite(deck, out Sprite fillSp) ? fillSp : GetWhiteUnitSprite();
        if (fillImage.sprite == null)
        {
            fillImage.sprite = GetWhiteUnitSprite();
        }

        fillImage.type = Image.Type.Filled;
        fillImage.fillMethod = Image.FillMethod.Horizontal;
        fillImage.fillOrigin = (int)Image.OriginHorizontal.Left;
        fillImage.color = Color.white;
        fillImage.fillAmount = 1f;
        fillImage.raycastTarget = false;

        var chromeLineColorers = new FFDataDeckUIColorer[borderColorers.Length + 1];
        Array.Copy(borderColorers, chromeLineColorers, borderColorers.Length);
        chromeLineColorers[^1] = dividerColorer;

        return new DeckParts(
            chromeGo,
            iconImage,
            label,
            fillImage,
            trackImage,
            chromeLineColorers);
    }

    private static Font ResolveUiFont()
    {
        Font f = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (f != null)
        {
            return f;
        }

        f = Resources.GetBuiltinResource<Font>("Arial.ttf");
        if (f != null)
        {
            return f;
        }

        return Font.CreateDynamicFontFromOSFont("Arial", 16);
    }

    private static RectTransform AddChromeInner(Transform chromeTransform, float inset)
    {
        var innerGo = CreateChild("Inner", chromeTransform);
        var innerRt = innerGo.AddComponent<RectTransform>();
        innerRt.anchorMin = Vector2.zero;
        innerRt.anchorMax = Vector2.one;
        innerRt.offsetMin = new Vector2(inset, inset);
        innerRt.offsetMax = new Vector2(-inset, -inset);
        return innerRt;
    }

    private static FFDataDeckUIColorer[] AddChromeBorderStrips(Transform chromeTransform, float bw, FFDataDeck deck)
    {
        return new[]
        {
            AddBorderStrip(chromeTransform, deck, "BorderTop",
                new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, bw)),
            AddBorderStrip(chromeTransform, deck, "BorderBottom",
                new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, bw)),
            AddBorderStrip(chromeTransform, deck, "BorderLeft",
                new Vector2(0f, 0f), new Vector2(0f, 1f), new Vector2(0f, 0.5f), new Vector2(bw, 0f)),
            AddBorderStrip(chromeTransform, deck, "BorderRight",
                new Vector2(1f, 0f), new Vector2(1f, 1f), new Vector2(1f, 0.5f), new Vector2(bw, 0f)),
        };
    }

    private static FFDataDeckUIColorer AddBorderStrip(
        Transform parent,
        FFDataDeck deck,
        string name,
        Vector2 anchorMin,
        Vector2 anchorMax,
        Vector2 pivot,
        Vector2 sizeDelta)
    {
        var go = CreateChild(name, parent);
        var rt = go.AddComponent<RectTransform>();
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.pivot = pivot;
        rt.anchoredPosition = Vector2.zero;
        rt.sizeDelta = sizeDelta;
        var img = go.AddComponent<Image>();
        img.sprite = GetWhiteUnitSprite();
        img.color = Color.white;
        img.raycastTarget = false;
        return DeckTint.BindImage(img, deck, false);
    }

    private static GameObject CreateChild(string name, Transform parent)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        return go;
    }

    private static Sprite GetWhiteUnitSprite()
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

/// <summary>From <see cref="DeckPanel.Build"/>.</summary>
internal readonly struct DeckParts
{
    public DeckParts(
        GameObject chromeRoot,
        Image icon,
        Text label,
        Image fill,
        Image track,
        FFDataDeckUIColorer[] lines)
    {
        ChromeRoot = chromeRoot;
        Icon = icon;
        Label = label;
        Fill = fill;
        Track = track;
        LineColorers = lines;
    }

    public GameObject ChromeRoot { get; }
    public Image Icon { get; }
    public Text Label { get; }
    public Image Fill { get; }
    public Image Track { get; }
    public FFDataDeckUIColorer[] LineColorers { get; }
}
