using FairyDust.Hud.Configuration;
using FairyDust.Hud.Modules.Hud;
using FairyDust.Hud.Modules.Hud.Interop;
using UnityEngine.UI;

namespace FairyDust.Hud.Modules.BleedOut;

/// <summary>Placeholder module for future bleed-out UI; toggled via <see cref="ModConfiguration.BleedOutModuleEnabled"/>.</summary>
internal sealed class BleedOutHudModule : IHudModule
{
    private HudModuleContext context;

    public string SlotObjectName => "Slot_BleedOut";

    public void OnAttach(HudModuleContext ctx) => context = ctx;

    public void OnSceneWasLoaded(int buildIndex, string sceneName)
    {
    }

    public void OnLateUpdate()
    {
        if (context == null)
        {
            return;
        }

        if (!Config.Values.BleedOutModuleEnabled)
        {
            context.Slot.gameObject.SetActive(false);
            return;
        }

        bool inWorld = FairyLocalPlayer.Current != null
            && FairyLocalPlayer.Current.gameObject.activeInHierarchy;

        context.Slot.gameObject.SetActive(inWorld);
        var le = context.Slot.GetComponent<LayoutElement>();
        le.preferredWidth = 0f;
        le.preferredHeight = 0f;
    }
}
