using FairyDust.Hud.Components;
using FairyDust.Hud.Components.Deck;
using Il2Cppmadeinfairyland.forsakenfrontiers.actor.player;
using Il2Cppmadeinfairyland.forsakenfrontiers.actor.player.datadeck;
using Il2CppTMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FairyDust.Hud.Modules.Stamina;

/// <summary><see cref="LineFrame"/> shell, <see cref="FillBar.Slim"/>, “STAMINA” label, optional status icon.</summary>
internal sealed class StaminaHudPanel
{
    public const float LinePx = 2f;
    public const float Pad = 6f;
    public const float ColGap = 6f;
    public const float BarW = 200f;
    public const float BarH = 18f;
    /// <summary>Target font height budget for <see cref="CopyDataDeckTextStyle"/> scaling; actual row uses TMP preferred size.</summary>
    public const float LabelH = 19f;
    private const float RedZoneEnd = 0.2f;
    /// <summary>Uniform inset from the icon square edges to the sprite (keeps icon centered in cell).</summary>
    private const float IconCellInset = 4f;
    private static readonly float RightW = BarW;
    /// <summary>Inner row: room for one TMP line + bar; left cell is a square of this size.</summary>
    private static readonly float RowInnerH = 24f + BarH;
    private static readonly float IconCellSide = RowInnerH;
    private static readonly float InnerW = Pad + IconCellSide + LinePx + ColGap + RightW + Pad;
    private static readonly float InnerH = Pad + RowInnerH + Pad;
    private static readonly Vector2 Outer = new(InnerW + 2f * LinePx, InnerH + 2f * LinePx);

    public static Vector2 PreferredOuterSize => Outer;

    private GameObject panelRoot;
    private FFDataDeck lastDeck;
    private Image iconImage;
    private FillBarHandle bar;

    public void EnsureBuilt(RectTransform slot, FFDataDeck deck, FFPlayer player)
    {
        if (player == null)
        {
            return;
        }

        if (panelRoot != null
            && panelRoot.transform.parent == slot.transform
            && ReferenceEquals(lastDeck, deck)
            && bar.FillRect != null)
        {
            return;
        }

        Teardown();
        lastDeck = deck;

        panelRoot = new GameObject("StaminaPanel");
        panelRoot.transform.SetParent(slot.transform, false);
        var pr = panelRoot.AddComponent<RectTransform>();
        pr.anchorMin = Vector2.zero;
        pr.anchorMax = Vector2.one;
        pr.offsetMin = Vector2.zero;
        pr.offsetMax = Vector2.zero;

        var box = new GameObject("StaminaFramed");
        box.transform.SetParent(panelRoot.transform, false);
        var boxRt = box.AddComponent<RectTransform>();
        boxRt.anchorMin = new Vector2(0f, 0f);
        boxRt.anchorMax = new Vector2(0f, 0f);
        boxRt.pivot = new Vector2(0f, 0f);
        boxRt.anchoredPosition = Vector2.zero;
        boxRt.sizeDelta = Outer;

        RectTransform content = LineFrame.Build(box.transform, LinePx, deck);

        var iconFrameGo = new GameObject("IconFrame");
        iconFrameGo.transform.SetParent(content, false);
        var iconFrameRt = iconFrameGo.AddComponent<RectTransform>();
        iconFrameRt.anchorMin = new Vector2(0f, 1f);
        iconFrameRt.anchorMax = new Vector2(0f, 1f);
        iconFrameRt.pivot = new Vector2(0f, 1f);
        iconFrameRt.anchoredPosition = new Vector2(Pad, -Pad);
        iconFrameRt.sizeDelta = new Vector2(IconCellSide, IconCellSide);

        var iconGo = new GameObject("Icon");
        iconGo.transform.SetParent(iconFrameGo.transform, false);
        var iconRt = iconGo.AddComponent<RectTransform>();
        float iconD = IconCellSide - 2f * IconCellInset;
        iconD = Mathf.Max(1f, iconD);
        iconRt.anchorMin = new Vector2(0.5f, 0.5f);
        iconRt.anchorMax = new Vector2(0.5f, 0.5f);
        iconRt.pivot = new Vector2(0.5f, 0.5f);
        iconRt.anchoredPosition = Vector2.zero;
        iconRt.sizeDelta = new Vector2(iconD, iconD);
        iconImage = iconGo.AddComponent<Image>();
        iconImage.preserveAspect = true;
        iconImage.raycastTarget = false;
        if (deck != null && StaminaHudBleedIcon.TryGetBleedSprite(deck, out Sprite sp))
        {
            iconImage.sprite = sp;
            iconImage.color = Color.white;
        }
        else
        {
            iconImage.color = new Color(1f, 1f, 1f, 0f);
        }

        LineFrame.VLine(content, Pad + IconCellSide, LinePx, deck);

        var rightGo = new GameObject("StaminaBlock");
        rightGo.transform.SetParent(content, false);
        var rightRt = rightGo.AddComponent<RectTransform>();
        rightRt.anchorMin = new Vector2(0f, 1f);
        rightRt.anchorMax = new Vector2(0f, 1f);
        rightRt.pivot = new Vector2(0f, 1f);
        float x = Pad + IconCellSide + LinePx + ColGap;
        rightRt.anchoredPosition = new Vector2(x, -Pad);
        rightRt.sizeDelta = new Vector2(RightW, RowInnerH);
        var v = rightGo.AddComponent<VerticalLayoutGroup>();
        v.childAlignment = TextAnchor.UpperLeft;
        v.spacing = 0f;
        v.padding = new RectOffset(0, 0, 0, 0);
        v.childControlWidth = true;
        v.childControlHeight = true;
        v.childForceExpandWidth = true;
        v.childForceExpandHeight = false;

        var labelGo = new GameObject("Label");
        labelGo.transform.SetParent(rightGo.transform, false);
        var labelRt = labelGo.AddComponent<RectTransform>();
        labelRt.anchorMin = new Vector2(0f, 1f);
        labelRt.anchorMax = new Vector2(1f, 1f);
        labelRt.pivot = new Vector2(0f, 1f);
        labelRt.anchoredPosition = Vector2.zero;
        labelRt.sizeDelta = new Vector2(0f, 0f);
        if (deck != null)
        {
            var label = labelGo.AddComponent<TextMeshProUGUI>();
            label.text = "STAMINA";
            label.raycastTarget = false;
            label.enableWordWrapping = false;
            label.overflowMode = TextOverflowModes.Overflow;
            label.margin = new Vector4(0f, 0f, 0f, 0f);
            CopyDataDeckTextStyle(label, deck);
            DeckTint.BindText(label, deck, false);
        }
        else
        {
            var label = labelGo.AddComponent<Text>();
            label.text = "STAMINA";
            label.font = ResolveUiFont();
            label.fontSize = 12;
            label.fontStyle = FontStyle.Bold;
            label.color = Color.white;
            label.alignment = TextAnchor.UpperLeft;
            label.horizontalOverflow = HorizontalWrapMode.Overflow;
            label.verticalOverflow = VerticalWrapMode.Truncate;
            label.raycastTarget = false;
        }

        var leLabel = labelGo.AddComponent<LayoutElement>();
        leLabel.flexibleHeight = 0f;
        leLabel.minHeight = 0f;
        var labelCsf = labelGo.AddComponent<ContentSizeFitter>();
        labelCsf.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
        labelCsf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        var barGo = new GameObject("Bar");
        barGo.transform.SetParent(rightGo.transform, false);
        var barRt = barGo.AddComponent<RectTransform>();
        barRt.anchorMin = new Vector2(0f, 1f);
        barRt.anchorMax = new Vector2(1f, 1f);
        barRt.pivot = new Vector2(0.5f, 1f);
        barRt.anchoredPosition = Vector2.zero;
        barRt.sizeDelta = new Vector2(0f, BarH);
        var leBar = barGo.AddComponent<LayoutElement>();
        leBar.minHeight = BarH;
        leBar.preferredHeight = BarH;
        leBar.flexibleHeight = 0f;

        LayoutRebuilder.ForceRebuildLayoutImmediate(rightRt);
        bar = FillBar.Slim(barRt.transform);
    }

    /// <summary>
    /// Match Data Deck: TMP SDF on <c>FFDataDeck</c> (see probe <c>.../BG/game time</c> / stats labels).
    /// </summary>
    private static void CopyDataDeckTextStyle(TextMeshProUGUI label, FFDataDeck deck)
    {
        TextMeshProUGUI src = deck.gameTime
            ?? deck.characterName
            ?? deck.credits
            ?? deck.flavorText;
        if (src == null)
        {
            label.fontSize = 16f;
            label.alignment = TextAlignmentOptions.TopLeft;
            return;
        }

        label.font = src.font;
        if (src.fontSharedMaterial != null)
        {
            label.fontSharedMaterial = src.fontSharedMaterial;
        }

        label.fontStyle = src.fontStyle;
        label.fontWeight = src.fontWeight;
        label.characterSpacing = src.characterSpacing;
        label.wordSpacing = src.wordSpacing;
        float srcH = 40f;
        if (src.rectTransform != null)
        {
            srcH = Mathf.Max(1f, src.rectTransform.sizeDelta.y);
        }

        label.fontSize = Mathf.Clamp(src.fontSize * (LabelH / srcH), 10f, 24f);
        label.alignment = TextAlignmentOptions.TopLeft;
        label.lineSpacing = 0f;
        label.paragraphSpacing = 0f;
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

    public void SetVisible(bool visible)
    {
        if (panelRoot != null)
        {
            panelRoot.SetActive(visible);
        }
    }

    public void UpdateVisuals(float t, FFDataDeck deck, FFPlayer player)
    {
        if (player == null)
        {
            return;
        }

        bar.SetRatio(t);
        if (bar.Fill != null)
        {
            float r = Mathf.Clamp01(t);
            bar.Fill.color = r < RedZoneEnd
                ? new Color(0.95f, 0.28f, 0.28f, 1f)
                : Color.white;
        }

        if (iconImage != null && deck != null && StaminaHudBleedIcon.TryGetBleedSprite(deck, out Sprite bleed))
        {
            iconImage.sprite = bleed;
            iconImage.color = Color.white;
        }
    }

    public void Teardown()
    {
        if (panelRoot != null)
        {
            UnityEngine.Object.Destroy(panelRoot);
            panelRoot = null;
        }

        lastDeck = null;
        iconImage = null;
        bar = default;
    }
}
