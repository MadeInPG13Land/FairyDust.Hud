using FairyDust.Hud.Modules.Status.Entities;
using FairyDust.Hud.Modules.Status.Services;

namespace FairyDust.Hud.Modules.Status.Bleed;

internal sealed class BleedStatusModule
{
    public IStatusProvider CreateProvider(PlayerStatusReadService reads, StatusFormatterService formatter) =>
        new BleedStatusProvider(reads, formatter);
}
