using FairyDust.Hud.Configuration;
using FairyDust.Hud.Modules.Hud;
using FairyDust.Hud.Modules.Status;
using MelonLoader;

namespace FairyDust.Hud;

public sealed class Main : MelonMod
{
    private static Main activeInstance;
    private GameplayHudHost hudHost;
    private bool loggedLateUpdateFailure;

    public override void OnInitializeMelon()
    {
        if (activeInstance != null && activeInstance != this)
        {
            activeInstance.hudHost?.Shutdown();
        }

        activeInstance = this;
        Config.Initialize();

        hudHost = new GameplayHudHost(this);
        hudHost.Register(new StatusHudModule());
        hudHost.Initialize();

        LoggerInstance.Msg($"{Metadata.Name} v{Metadata.Version} - {Config.FilePath}");
        LoggerInstance.Msg("HUD config loaded: " + Config.LoadedValuesSummary);
    }

    public override void OnSceneWasLoaded(int buildIndex, string sceneName) =>
        hudHost?.OnSceneWasLoaded(buildIndex, sceneName);

    public override void OnLateUpdate()
    {
        try
        {
            hudHost?.OnLateUpdate();
        }
        catch (Exception ex)
        {
            if (!loggedLateUpdateFailure)
            {
                loggedLateUpdateFailure = true;
                LoggerInstance.Error("HUD OnLateUpdate failed: " + ex);
            }
        }
    }
}
