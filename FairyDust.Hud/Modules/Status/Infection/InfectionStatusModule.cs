using FairyDust.Hud.Modules.Status.Entities;
using FairyDust.Hud.Modules.Status.Services;

namespace FairyDust.Hud.Modules.Status.Infection;

internal sealed class InfectionStatusModule
{
    public IStatusProvider CreateProvider(PlayerStatusReadService reads, StatusFormatterService formatter) =>
        new InfectionStatusProvider(reads, formatter);
}
