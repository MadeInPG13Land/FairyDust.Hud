using MelonLoader;
using UnityEngine;

namespace FairyDust.Hud.Modules.Hud;

/// <summary>Per-module attachment: layout slot under the shared dock and access to the Melon host.</summary>
public sealed class HudModuleContext
{
    public HudModuleContext(RectTransform slot, MelonMod hostMod)
    {
        Slot = slot;
        HostMod = hostMod;
    }

    public RectTransform Slot
    {
        get;
    }

    public MelonMod HostMod
    {
        get;
    }
}
