using FairyDust.Hud.Configuration;
using FairyDust.Hud.Modules.Status.Entities;
using FairyDust.Hud.Modules.Status.Services;
using UnityEngine;

namespace FairyDust.Hud.Modules.Status.Infection;

internal sealed class InfectionStatusProvider : IStatusProvider
{
    private const float LowWarningRatio = 0.25f;
    private readonly PlayerStatusReadService reads;
    private readonly StatusFormatterService formatter;

    public InfectionStatusProvider(PlayerStatusReadService reads, StatusFormatterService formatter)
    {
        this.reads = reads;
        this.formatter = formatter;
    }

    public string Id => "infection";

    public int SortOrder => 10;

    public bool IsEnabled => Config.Values.InfectionModuleEnabled;

    public bool TryGetRow(StatusProviderContext context, out StatusRowSnapshot row)
    {
        row = default;
        bool infected = reads.IsInfected(context.Player);
        if (!infected)
        {
            return false;
        }

        float total = Mathf.Max(reads.InfectionTakeoverDuration(context.Player), 1e-5f);
        float timer = Mathf.Max(0f, reads.InfectionTimer(context.Player));
        float ratio = Mathf.Clamp01(timer / total);
        row = new StatusRowSnapshot(
            StatusRowKind.Infection,
            Id,
            "INFECTION",
            ratio,
            SortOrder,
            formatter.Timer(timer),
            ratio <= LowWarningRatio);
        return true;
    }
}
