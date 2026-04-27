using FairyDust.Hud.Configuration;
using FairyDust.Hud.Modules.Status.Entities;
using FairyDust.Hud.Modules.Status.Services;
using UnityEngine;

namespace FairyDust.Hud.Modules.Status.Stamina;

internal sealed class StaminaStatusProvider : IStatusProvider
{
    private const float LowWarningRatio = 0.25f;
    private readonly PlayerStatusReadService reads;

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
        row = new StatusRowSnapshot(
            StatusRowKind.Stamina,
            Id,
            "STAMINA",
            staminaRatio,
            SortOrder,
            warning: staminaRatio <= LowWarningRatio);
        return true;
    }
}
