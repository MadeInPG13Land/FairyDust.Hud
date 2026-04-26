using FairyDust.Hud.Configuration;
using FairyDust.Hud.Modules.Hud;
using FairyDust.Hud.Modules.Hud.Interop;
using UnityEngine;
using UnityEngine.UI;

namespace FairyDust.Hud.Modules.Stamina;

internal sealed class StaminaHudModule : IHudModule
{
    private HudModuleContext context;
    private readonly StaminaHudPanel panel = new();

    public string SlotObjectName => "Slot_Stamina";

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

        if (!Config.Values.StaminaModuleEnabled)
        {
            panel.SetVisible(false);
            context.Slot.gameObject.SetActive(false);
            return;
        }

        var player = FairyLocalPlayer.Current;
        if (player is null || !player.gameObject.activeInHierarchy)
        {
            panel.SetVisible(false);
            context.Slot.gameObject.SetActive(false);
            return;
        }

        context.Slot.gameObject.SetActive(true);
        ApplySlotSize();

        panel.EnsureBuilt(context.Slot, player.DataDeck, player);
        panel.SetVisible(true);

        float max = Mathf.Max(player.MaxStamina, 1e-5f);
        float ratio = Mathf.Clamp01(player.Stamina / max);
        panel.UpdateVisuals(ratio, player.DataDeck, player);
    }

    private void ApplySlotSize()
    {
        var le = context.Slot.GetComponent<LayoutElement>();
        Vector2 s = StaminaHudPanel.PreferredOuterSize;
        le.preferredWidth = s.x;
        le.preferredHeight = s.y;

        // Baseline: give the slot a real rect immediately (anchors 0,0 pivot 0,0).
        context.Slot.sizeDelta = s;

        if (context.Slot.parent is RectTransform dockRt)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(dockRt);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(context.Slot);
    }
}
