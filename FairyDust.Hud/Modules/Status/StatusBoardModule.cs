using FairyDust.Hud.Components.Deck;
using FairyDust.Hud.Configuration;
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
    private const float NormalBackgroundAlpha = 0f;
    private const float MinimumMeaningfulTimer = 0.01f;
    private const float TimerUpperBoundTolerance = 1f;
    private const float LowWarningRatio = 0.25f;

    private readonly PlayerStatusReadService reads;
    private readonly StatusFormatterService formatter;
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
        formatter = new StatusFormatterService(reads);
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

            if (PlayerHudStateService.IsCurrentPlayerIncapacitated)
            {
                CollectDownedRows(player, rowBuffer);
            }
            else
            {
                board.CollectRows(new StatusProviderContext(player, deck), rowBuffer);
            }

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

    private void CollectDownedRows(Il2Cppmadeinfairyland.forsakenfrontiers.actor.player.FFPlayer player, List<StatusRowSnapshot> rows)
    {
        rows.Clear();
        if (TryGetMostUrgentDownedRow(player, out StatusRowSnapshot row))
        {
            rows.Add(row);
        }
    }

    private bool TryGetMostUrgentDownedRow(
        Il2Cppmadeinfairyland.forsakenfrontiers.actor.player.FFPlayer player,
        out StatusRowSnapshot row)
    {
        row = default;
        bool hasCandidate = false;
        float bestSeconds = float.MaxValue;
        StatusRowSnapshot bestRow = default;

        ConsiderBleed(player, ref hasCandidate, ref bestSeconds, ref bestRow);
        ConsiderInfection(player, ref hasCandidate, ref bestSeconds, ref bestRow);
        ConsiderFrostbite(player, ref hasCandidate, ref bestSeconds, ref bestRow);

        if (!hasCandidate)
        {
            return false;
        }

        row = bestRow;
        return true;
    }

    private void ConsiderBleed(
        Il2Cppmadeinfairyland.forsakenfrontiers.actor.player.FFPlayer player,
        ref bool hasCandidate,
        ref float bestSeconds,
        ref StatusRowSnapshot bestRow)
    {
        if (!Config.Values.BleedOutModuleEnabled || !reads.HasActiveBleedOut(player, out float timer, out float total))
        {
            return;
        }

        ConsiderCandidate(
            StatusRowKind.Bleed,
            "bleed",
            "BLEED",
            timer,
            Mathf.Clamp01(timer / Mathf.Max(total, MinimumMeaningfulTimer)),
            total,
            ref hasCandidate,
            ref bestSeconds,
            ref bestRow);
    }

    private void ConsiderInfection(
        Il2Cppmadeinfairyland.forsakenfrontiers.actor.player.FFPlayer player,
        ref bool hasCandidate,
        ref float bestSeconds,
        ref StatusRowSnapshot bestRow)
    {
        if (!Config.Values.InfectionModuleEnabled || !reads.IsInfected(player))
        {
            return;
        }

        float total = reads.InfectionTakeoverDuration(player);
        float timer = reads.InfectionTimer(player);
        if (!IsMeaningfulCountdown(timer, total))
        {
            return;
        }

        ConsiderCandidate(
            StatusRowKind.Infection,
            "infection",
            "INFECTION",
            timer,
            Mathf.Clamp01(timer / total),
            total,
            ref hasCandidate,
            ref bestSeconds,
            ref bestRow);
    }

    private void ConsiderFrostbite(
        Il2Cppmadeinfairyland.forsakenfrontiers.actor.player.FFPlayer player,
        ref bool hasCandidate,
        ref float bestSeconds,
        ref StatusRowSnapshot bestRow)
    {
        if (!Config.Values.FrostbiteModuleEnabled)
        {
            return;
        }

        float max = Mathf.Max(reads.MaxFrostbite(player), MinimumMeaningfulTimer);
        float current = Mathf.Clamp(reads.Frostbite(player), 0f, max);
        float speed = reads.FrostbiteSpeed(player);
        bool active = reads.IsFrostbiteActive(player);
        if (!active
            || reads.NearWarmth(player)
            || reads.HasWarmthBenefits(player)
            || speed <= MinimumMeaningfulTimer
            || current <= MinimumMeaningfulTimer
            || current >= max)
        {
            return;
        }

        float timer = (max - current) / speed;
        ConsiderCandidate(
            StatusRowKind.Frostbite,
            "frostbite",
            "FROSTBITE",
            timer,
            Mathf.Clamp01(current / max),
            max / speed,
            ref hasCandidate,
            ref bestSeconds,
            ref bestRow);
    }

    private void ConsiderCandidate(
        StatusRowKind kind,
        string id,
        string label,
        float seconds,
        float ratio,
        float totalSeconds,
        ref bool hasCandidate,
        ref float bestSeconds,
        ref StatusRowSnapshot bestRow)
    {
        if (seconds <= MinimumMeaningfulTimer || seconds >= bestSeconds)
        {
            return;
        }

        hasCandidate = true;
        bestSeconds = seconds;
        bestRow = new StatusRowSnapshot(
            kind,
            id,
            label,
            ratio,
            -100,
            formatter.Timer(seconds),
            totalSeconds > MinimumMeaningfulTimer && seconds / totalSeconds <= LowWarningRatio);
    }

    private static bool IsMeaningfulCountdown(float timer, float total)
    {
        return total > MinimumMeaningfulTimer
            && timer > MinimumMeaningfulTimer
            && timer <= total + TimerUpperBoundTolerance;
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
