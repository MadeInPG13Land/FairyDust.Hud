using FairyDust.Hud.Components.Deck;
using FairyDust.Hud.Configuration;
using FairyDust.Hud.Modules.Hud;
using FairyDust.Hud.Modules.Hud.Interop;
using Il2Cppmadeinfairyland.forsakenfrontiers.actor.player;
using Il2Cppmadeinfairyland.forsakenfrontiers.actor.player.datadeck;
using MelonLoader;
using UnityEngine;
using UnityEngine.UI;

namespace FairyDust.Hud.Modules.Status;

internal sealed class StatusHudModule : IHudModule
{
    private static readonly TimeSpan TextStyleRefreshInterval = TimeSpan.FromSeconds(1);
    private const int BleedRowIndex = 0;
    private const int InfectionRowIndex = 1;
    private const int FrostbiteRowIndex = 2;
    private const int StaminaRowIndex = 3;
    private const float LowWarningRatio = 0.25f;
    private const float HighWarningRatio = 0.75f;
    private const float NormalBackgroundAlpha = 0.15f;

    private HudModuleContext context;
    private DeckStatusPanelHandle panel;
    private FFDataDeck lastDeck;
    private bool loggedUpdateFailure;
    private bool loggedPanelBuildFailure;
    private bool loggedInvalidStamina;
    private bool loggedInvalidBleed;
    private bool loggedInvalidInfection;
    private bool loggedInvalidFrostbite;
    private DateTime nextTextStyleRefreshUtc = DateTime.MinValue;

    public string SlotObjectName => "Slot_StatusBoard";

    public void OnAttach(HudModuleContext ctx) => context = ctx;

    public void OnSceneWasLoaded(int buildIndex, string sceneName)
    {
        TeardownPanel();
    }

    public void OnLateUpdate()
    {
        if (context?.Slot == null)
        {
            return;
        }

        string phase = "start";
        try
        {
            phase = "read local player";
            var player = FairyLocalPlayer.Current;
            if (!IsUsable(player) || !AnyStatusRowEnabled())
            {
                SafeSetVisible(false);
                return;
            }

            phase = "read datadeck";
            FFDataDeck deck = SafeDataDeck(player);
            if (SafeDataDeckOpen(deck))
            {
                SafeSetVisible(false);
                return;
            }

            phase = "activate slot";
            context.Slot.gameObject.SetActive(true);
            phase = "initial slot size";
            ApplySlotSize(VisibleRowCount(player));

            phase = "ensure panel";
            EnsureBuilt(context.Slot, deck);
            phase = "show panel";
            panel.SetVisible(true);
            panel.SetBackgroundAlpha(NormalBackgroundAlpha);
            phase = "refresh flavor";
            RefreshDeckFlavor(deck);
            phase = "update rows";
            UpdateRows(player);
            phase = "apply visible layout";
            ApplySlotSize(panel.ApplyVisibleLayout());
            loggedUpdateFailure = false;
        }
        catch (Exception ex)
        {
            SafeSetVisible(false);
            if (!loggedUpdateFailure)
            {
                loggedUpdateFailure = true;
                MelonLogger.Warning("[FairyDust.Hud] Status board update failed during '" + phase + "': " + ex);
            }
        }
    }

    private static bool AnyStatusRowEnabled() =>
        Config.Values.StaminaModuleEnabled
        || Config.Values.BleedOutModuleEnabled
        || Config.Values.InfectionModuleEnabled
        || Config.Values.FrostbiteModuleEnabled;

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
        panel = DeckStatusPanel.Build(slot, deck, "StatusBoard");
        if (!panel.IsValid && !loggedPanelBuildFailure)
        {
            loggedPanelBuildFailure = true;
            MelonLogger.Warning("[FairyDust.Hud] Status board build returned an invalid handle.");
        }
    }

    private void UpdateRows(FFPlayer player)
    {
        float staminaMax = Mathf.Max(SafeFloat(() => player.MaxStamina), 1e-5f);
        float staminaRatio = Config.Values.StaminaModuleEnabled
            ? Mathf.Clamp01(SafeFloat(() => player.Stamina) / staminaMax)
            : 0f;
        if (Config.Values.StaminaModuleEnabled && (staminaRatio < 0f || staminaRatio > 1f || float.IsNaN(staminaRatio)) && !loggedInvalidStamina)
        {
            loggedInvalidStamina = true;
            MelonLogger.Warning("[FairyDust.Hud] Unexpected stamina ratio: " + staminaRatio);
        }

        bool bleeding = Config.Values.BleedOutModuleEnabled && SafeBool(() => player.IsBleeding);
        float bleedTotal = Mathf.Max(SafeFloat(() => player.BleedToDeathTime), 1e-5f);
        float bleedTimer = Mathf.Max(0f, SafeFloat(() => player.BleedToDeathTimer));
        float bleedRatio = bleeding ? Mathf.Clamp01(bleedTimer / bleedTotal) : 0f;
        if (bleeding && bleedTimer > bleedTotal + 1f && !loggedInvalidBleed)
        {
            loggedInvalidBleed = true;
            MelonLogger.Warning($"[FairyDust.Hud] Bleed timer exceeds total: timer={bleedTimer:0.00}, total={bleedTotal:0.00}");
        }
        panel.SetRowVisible(BleedRowIndex, bleeding);
        if (bleeding)
        {
            panel.SetRow(
            BleedRowIndex,
            "BLEED",
            bleedRatio,
            bleeding ? FormatTimer(bleedTimer) : null,
            bleeding && bleedRatio <= LowWarningRatio);
        }

        bool infected = Config.Values.InfectionModuleEnabled && SafeBool(() => player.Infected);
        float infectionTotal = Mathf.Max(SafeFloat(() => player.InfectionTakeoverDuration), 1e-5f);
        float infectionTimer = Mathf.Max(0f, SafeFloat(() => player.InfectionTimer));
        float infectionRatio = infected ? Mathf.Clamp01(infectionTimer / infectionTotal) : 0f;
        if (infected && infectionTimer > infectionTotal + 1f && !loggedInvalidInfection)
        {
            loggedInvalidInfection = true;
            MelonLogger.Warning($"[FairyDust.Hud] Infection timer exceeds total: timer={infectionTimer:0.00}, total={infectionTotal:0.00}");
        }
        panel.SetRowVisible(InfectionRowIndex, infected);
        if (infected)
        {
            panel.SetRow(
            InfectionRowIndex,
            "INFECTION",
            infectionRatio,
            infected ? FormatTimer(infectionTimer) : null,
            infected && infectionRatio <= LowWarningRatio);
        }

        float frostbiteMax = Mathf.Max(SafeFloat(() => player.maxFrostbite), 1e-5f);
        float frostbite = Mathf.Clamp(SafeFloat(() => player.Frostbite), 0f, frostbiteMax);
        float frostbiteRatio = Mathf.Clamp01(frostbite / frostbiteMax);
        bool frostbiteActive = Config.Values.FrostbiteModuleEnabled
            && (SafeBool(() => player.FrostbiteActive) || frostbite > 0.01f);
        if (Config.Values.FrostbiteModuleEnabled && (float.IsNaN(frostbiteRatio) || frostbite > frostbiteMax + 1f) && !loggedInvalidFrostbite)
        {
            loggedInvalidFrostbite = true;
            MelonLogger.Warning($"[FairyDust.Hud] Unexpected frostbite value: frostbite={frostbite:0.00}, max={frostbiteMax:0.00}, ratio={frostbiteRatio}");
        }
        panel.SetRowVisible(FrostbiteRowIndex, frostbiteActive);
        if (frostbiteActive)
        {
            panel.SetRow(
                FrostbiteRowIndex,
                "FROSTBITE",
                frostbiteRatio,
                FormatFrostbiteValue(player, frostbite, frostbiteMax),
                frostbiteRatio >= HighWarningRatio);
        }

        panel.SetRowVisible(StaminaRowIndex, Config.Values.StaminaModuleEnabled);
        panel.SetRow(
            StaminaRowIndex,
            "STAMINA",
            staminaRatio,
            lowWarning: Config.Values.StaminaModuleEnabled && staminaRatio <= LowWarningRatio);
    }

    private void ApplySlotSize(int visibleRowCount)
    {
        var le = context.Slot.GetComponent<LayoutElement>();
        if (le == null)
        {
            le = context.Slot.gameObject.AddComponent<LayoutElement>();
            MelonLogger.Warning("[FairyDust.Hud] Status board slot was missing LayoutElement; added one.");
        }

        Vector2 size = DeckStatusPanel.OuterSizeForRows(visibleRowCount);
        le.preferredWidth = size.x;
        le.preferredHeight = size.y;
        context.Slot.sizeDelta = size;

        if (context.Slot.parent is RectTransform dockRt)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(dockRt);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(context.Slot);
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
        catch (Exception ex)
        {
            if (!loggedUpdateFailure)
            {
                MelonLogger.Warning("[FairyDust.Hud] Status board visibility update failed: " + ex);
            }
        }
    }

    private void TeardownPanel()
    {
        panel.Destroy();
        panel = default;
        lastDeck = null;
        nextTextStyleRefreshUtc = DateTime.MinValue;
    }

    private static int VisibleRowCount(FFPlayer player)
    {
        int count = Config.Values.StaminaModuleEnabled ? 1 : 0;
        if (Config.Values.BleedOutModuleEnabled && SafeBool(() => player.IsBleeding))
        {
            count++;
        }

        if (Config.Values.InfectionModuleEnabled && SafeBool(() => player.Infected))
        {
            count++;
        }

        if (Config.Values.FrostbiteModuleEnabled
            && (SafeBool(() => player.FrostbiteActive) || SafeFloat(() => player.Frostbite) > 0.01f))
        {
            count++;
        }

        return Mathf.Max(1, count);
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

    private static string FormatTimer(float seconds)
    {
        int totalSeconds = Mathf.Max(0, Mathf.CeilToInt(seconds));
        int minutes = totalSeconds / 60;
        int secs = totalSeconds % 60;
        return $"{minutes:00}:{secs:00}";
    }

    private static string FormatFrostbiteValue(FFPlayer player, float frostbite, float maxFrostbite)
    {
        float speed = SafeFloat(() => player.frostbiteSpeed);
        bool active = SafeBool(() => player.FrostbiteActive);
        if (active && speed > 0.001f && frostbite < maxFrostbite)
        {
            return FormatTimer((maxFrostbite - frostbite) / speed);
        }

        return Mathf.RoundToInt(Mathf.Clamp01(frostbite / Mathf.Max(maxFrostbite, 1e-5f)) * 100f) + "%";
    }

    private static bool IsUsable(FFPlayer player)
    {
        if (player == null)
        {
            return false;
        }

        try
        {
            return player.gameObject != null && player.gameObject.activeInHierarchy;
        }
        catch
        {
            return false;
        }
    }

    private static FFDataDeck SafeDataDeck(FFPlayer player)
    {
        try
        {
            return player != null ? player.DataDeck : null;
        }
        catch
        {
            return null;
        }
    }

    private static bool SafeDataDeckOpen(FFDataDeck deck)
    {
        try
        {
            return deck != null && deck.DataDeckOpen;
        }
        catch
        {
            return false;
        }
    }

    private static float SafeFloat(Func<float> read)
    {
        try
        {
            return read();
        }
        catch
        {
            return 0f;
        }
    }

    private static bool SafeBool(Func<bool> read)
    {
        try
        {
            return read();
        }
        catch
        {
            return false;
        }
    }
}
