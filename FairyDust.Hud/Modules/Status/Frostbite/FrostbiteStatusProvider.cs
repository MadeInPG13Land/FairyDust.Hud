using FairyDust.Hud.Configuration;
using FairyDust.Hud.Modules.Status.Entities;
using FairyDust.Hud.Modules.Status.Services;
using UnityEngine;

namespace FairyDust.Hud.Modules.Status.Frostbite;

internal sealed class FrostbiteStatusProvider : IStatusProvider
{
    private const float HighWarningRatio = 0.75f;
    private readonly PlayerStatusReadService reads;
    private readonly StatusFormatterService formatter;

    public FrostbiteStatusProvider(PlayerStatusReadService reads, StatusFormatterService formatter)
    {
        this.reads = reads;
        this.formatter = formatter;
    }

    public string Id => "frostbite";

    public int SortOrder => 20;

    public bool IsEnabled => Config.Values.FrostbiteModuleEnabled;

    public bool TryGetRow(StatusProviderContext context, out StatusRowSnapshot row)
    {
        row = default;
        float max = Mathf.Max(reads.MaxFrostbite(context.Player), 1e-5f);
        float current = Mathf.Clamp(reads.Frostbite(context.Player), 0f, max);
        bool active = reads.IsFrostbiteActive(context.Player) || current > 0.01f;
        if (!active)
        {
            return false;
        }

        float ratio = Mathf.Clamp01(current / max);
        row = new StatusRowSnapshot(
            StatusRowKind.Frostbite,
            Id,
            "FROSTBITE",
            ratio,
            SortOrder,
            formatter.FrostbiteValue(context.Player, current, max),
            ratio >= HighWarningRatio);
        return true;
    }
}
