using FairyDust.Hud.Modules.Status.Entities;
using FairyDust.Hud.Modules.Status.Services;

namespace FairyDust.Hud.Modules.Status.Stamina;

internal sealed class StaminaStatusModule
{
    public IStatusProvider CreateProvider(PlayerStatusReadService reads, StatusFormatterService formatter) =>
        new StaminaStatusProvider(reads);
}
