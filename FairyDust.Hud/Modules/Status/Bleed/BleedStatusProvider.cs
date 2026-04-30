using FairyDust.Hud.Configuration;
using FairyDust.Hud.Modules.Status.Entities;
using FairyDust.Hud.Modules.Status.Services;
using UnityEngine;

namespace FairyDust.Hud.Modules.Status.Bleed;

internal sealed class BleedStatusProvider : IStatusProvider
{
    private const float LowWarningRatio = 0.25f;
    private readonly PlayerStatusReadService reads;
    private readonly StatusFormatterService formatter;

    public BleedStatusProvider(PlayerStatusReadService reads, StatusFormatterService formatter)
    {
        this.reads = reads;
        this.formatter = formatter;
    }

    public string Id => "bleed";

    public int SortOrder => 0;

    public bool IsEnabled => Config.Values.BleedOutModuleEnabled;

    public bool TryGetRow(StatusProviderContext context, out StatusRowSnapshot row)
    {
        row = default;
        if (!reads.HasActiveBleedOut(context.Player, out float timer, out float total))
        {
            return false;
        }

        float ratio = Mathf.Clamp01(timer / total);
        row = new StatusRowSnapshot(
            StatusRowKind.Bleed,
            Id,
            "BLEED",
            ratio,
            SortOrder,
            formatter.Timer(timer),
            ratio <= LowWarningRatio);
        return true;
    }
}
