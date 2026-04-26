namespace FairyDust.Hud.Modules.Hud;

/// <summary>One logical HUD block (stamina, bleed-out, etc.) with its own UI under an assigned slot.</summary>
public interface IHudModule
{
    /// <summary>Child name under the dock, e.g. <c>Slot_Stamina</c>.</summary>
    string SlotObjectName
    {
        get;
    }

    void OnAttach(HudModuleContext context);

    void OnSceneWasLoaded(int buildIndex, string sceneName);

    void OnLateUpdate();
}
