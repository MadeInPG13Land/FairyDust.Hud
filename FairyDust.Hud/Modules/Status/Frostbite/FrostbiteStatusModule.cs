using FairyDust.Hud.Modules.Status.Entities;
using FairyDust.Hud.Modules.Status.Services;

namespace FairyDust.Hud.Modules.Status.Frostbite;

internal sealed class FrostbiteStatusModule
{
    public IStatusProvider CreateProvider(PlayerStatusReadService reads, StatusFormatterService formatter) =>
        new FrostbiteStatusProvider(reads, formatter);
}
