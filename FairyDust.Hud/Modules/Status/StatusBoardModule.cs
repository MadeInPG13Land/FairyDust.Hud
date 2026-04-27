using FairyDust.Hud.Components.Deck;
using FairyDust.Hud.Game.Services;
using FairyDust.Hud.Host;
using FairyDust.Hud.Modules.Status.Bleed;
using FairyDust.Hud.Modules.Status.Entities;
using FairyDust.Hud.Modules.Status.Frostbite;
using FairyDust.Hud.Modules.Status.Infection;
using FairyDust.Hud.Modules.Status.Services;
using FairyDust.Hud.Modules.Status.Stamina;
using Il2Cppmadeinfairyland.forsakenfrontiers.actor.player.datadeck;
using UnityEngine;
using UnityEngine.UI;

namespace FairyDust.Hud.Modules.Status;

internal sealed class StatusBoardModule : IHudModule
{
    private static readonly TimeSpan TextStyleRefreshInterval = TimeSpan.FromSeconds(1);
    private const float NormalBackgroundAlpha = 0.15f;

    private readonly PlayerStatusReadService reads;
    private readonly StatusBoardService board;
    private readonly StatusVisibilityService visibility;
    private readonly List<StatusRowSnapshot> rowBuffer;

    private HudModuleContext context;
    private DeckStatusPanelHandle panel;
    private FFDataDeck lastDeck;
    private DateTime nextTextStyleRefreshUtc = DateTime.MinValue;
    private int lastAppliedVisibleRowCount = -1;
    private Vector2 lastAppliedSlotSize = new(float.NaN, float.NaN);

    public StatusBoardModule()
    {
        reads = new PlayerStatusReadService();
        var formatter = new StatusFormatterService(reads);
        board = new StatusBoardService(new IStatusProvider[]
        {
            new BleedStatusModule().CreateProvider(reads, formatter),
            new InfectionStatusModule().CreateProvider(reads, formatter),
            new FrostbiteStatusModule().CreateProvider(reads, formatter),
            new StaminaStatusModule().CreateProvider(reads, formatter),
        });
        visibility = new StatusVisibilityService(reads);
        rowBuffer = new List<StatusRowSnapshot>(board.Capacity);
    }

    public string SlotObjectName => "Slot_StatusBoard";

    public void OnAttach(HudModuleContext ctx) => context = ctx;

    public void OnSceneWasLoaded(int buildIndex, string sceneName) => TeardownPanel();

    public void OnLateUpdate()
    {
        if (context?.Slot == null)
        {
            return;
        }

        try
        {
            var player = FairyLocalPlayer.Current;
            FFDataDeck deck = reads.GetDataDeck(player);
            if (!visibility.ShouldShowBoard(player, deck, board.Providers))
            {
                SafeSetVisible(false);
                return;
            }

            board.CollectRows(new StatusProviderContext(player, deck), rowBuffer);
            if (rowBuffer.Count == 0)
            {
                SafeSetVisible(false);
                return;
            }

            if (!context.Slot.gameObject.activeSelf)
            {
                context.Slot.gameObject.SetActive(true);
            }

            EnsureBuilt(context.Slot, deck);
            panel.SetVisible(true);
            panel.SetBackgroundAlpha(NormalBackgroundAlpha);
            RefreshDeckFlavor(deck);
            ApplyRows(rowBuffer);
            ApplySlotSize(panel.ApplyVisibleLayout());
        }
        catch
        {
            SafeSetVisible(false);
        }
    }

    private void EnsureBuilt(RectTransform slot, FFDataDeck deck)
    {
        if (slot == null)
        {
            return;
        }

        if (panel.IsValid
            && panel.Root != null
            && panel.Root.transform.parent == slot.transform
            && lastDeck == deck)
        {
            return;
        }

        TeardownPanel();
        lastDeck = deck;
        panel = DeckStatusPanel.Build(slot, deck, "StatusBoard", board.Capacity);
    }

    private void ApplyRows(IReadOnlyList<StatusRowSnapshot> rows)
    {
        panel.ClearRows();
        for (int i = 0; i < rows.Count; i++)
        {
            StatusRowSnapshot row = rows[i];
            panel.SetRow(i, row.Label, row.Ratio, row.ValueText, row.Warning);
        }
    }

    private void ApplySlotSize(int visibleRowCount)
    {
        var le = context.Slot.GetComponent<LayoutElement>();
        if (le == null)
        {
            le = context.Slot.gameObject.AddComponent<LayoutElement>();
        }

        Vector2 size = DeckStatusPanel.OuterSizeForRows(visibleRowCount);
        if (visibleRowCount == lastAppliedVisibleRowCount && size == lastAppliedSlotSize)
        {
            return;
        }

        lastAppliedVisibleRowCount = visibleRowCount;
        lastAppliedSlotSize = size;
        le.preferredWidth = size.x;
        le.preferredHeight = size.y;
        context.Slot.sizeDelta = size;

        if (context.Slot.parent is RectTransform dockRt)
        {
            LayoutRebuilder.MarkLayoutForRebuild(dockRt);
        }

        LayoutRebuilder.MarkLayoutForRebuild(context.Slot);
    }

    private void SetVisible(bool visible)
    {
        panel.SetVisible(visible);
        if (context?.Slot != null)
        {
            context.Slot.gameObject.SetActive(visible);
        }
    }

    private void SafeSetVisible(bool visible)
    {
        try
        {
            SetVisible(visible);
        }
        catch
        {
        }
    }

    private void TeardownPanel()
    {
        panel.Destroy();
        panel = default;
        lastDeck = null;
        nextTextStyleRefreshUtc = DateTime.MinValue;
        lastAppliedVisibleRowCount = -1;
        lastAppliedSlotSize = new Vector2(float.NaN, float.NaN);
    }

    private void RefreshDeckFlavor(FFDataDeck deck)
    {
        if (deck == null || !panel.IsValid)
        {
            return;
        }

        DateTime now = DateTime.UtcNow;
        bool refreshTextStyle = now >= nextTextStyleRefreshUtc;
        if (refreshTextStyle)
        {
            nextTextStyleRefreshUtc = now + TextStyleRefreshInterval;
        }

        panel.RefreshFlavor(deck, refreshTextStyle);
    }
}
