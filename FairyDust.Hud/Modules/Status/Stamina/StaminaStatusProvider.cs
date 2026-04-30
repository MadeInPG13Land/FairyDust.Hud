using FairyDust.Hud.Configuration;
using FairyDust.Hud.Modules.Status.Entities;
using FairyDust.Hud.Modules.Status.Services;
using UnityEngine;

namespace FairyDust.Hud.Modules.Status.Stamina;

internal sealed class StaminaStatusProvider : IStatusProvider
{
    private const float LowWarningRatio = 0.25f;
    private const float FullRatioThreshold = 0.995f;
    private const float FullValueHoldSeconds = 0.75f;
    private readonly PlayerStatusReadService reads;
    private float hideValueAfterTime = float.NegativeInfinity;
    private bool valueVisible;

    public StaminaStatusProvider(PlayerStatusReadService reads)
    {
        this.reads = reads;
    }

    public string Id => "stamina";

    public int SortOrder => 30;

    public bool IsEnabled => Config.Values.StaminaModuleEnabled;

    public bool TryGetRow(StatusProviderContext context, out StatusRowSnapshot row)
    {
        float staminaMax = Mathf.Max(reads.MaxStamina(context.Player), 1e-5f);
        float staminaRatio = Mathf.Clamp01(reads.Stamina(context.Player) / staminaMax);
        string valueText = GetValueText(staminaRatio);
        row = new StatusRowSnapshot(
            StatusRowKind.Stamina,
            Id,
            "STAMINA",
            staminaRatio,
            SortOrder,
            valueText,
            warning: staminaRatio <= LowWarningRatio);
        return true;
    }

    private string GetValueText(float staminaRatio)
    {
        if (staminaRatio < FullRatioThreshold)
        {
            valueVisible = true;
            hideValueAfterTime = float.PositiveInfinity;
            return FormatPercent(staminaRatio);
        }

        if (valueVisible && float.IsPositiveInfinity(hideValueAfterTime))
        {
            hideValueAfterTime = Time.unscaledTime + FullValueHoldSeconds;
        }

        if (valueVisible && Time.unscaledTime < hideValueAfterTime)
        {
            return "100%";
        }

        valueVisible = false;
        hideValueAfterTime = float.NegativeInfinity;
        return null;
    }

    private static string FormatPercent(float ratio)
    {
        int percent = Mathf.Clamp(Mathf.RoundToInt(ratio * 100f), 0, 100);
        return percent + "%";
    }
}
