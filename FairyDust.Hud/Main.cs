using FairyDust.Hud.Configuration;
using FairyDust.Hud.Components.Deck;
using FairyDust.Hud.Game.Services;
using FairyDust.Hud.Host;
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
            activeInstance.hudHost = null;
        }

        activeInstance = this;
        Config.Initialize();
        HarmonyInstance.PatchAll(typeof(Main).Assembly);

        hudHost = new GameplayHudHost(this);
        hudHost.Register(new StatusBoardModule());
        hudHost.Initialize();

        LoggerInstance.Msg(ConfigConsoleCard.Build());
    }

    public override void OnDeinitializeMelon()
    {
        hudHost?.Shutdown();
        hudHost = null;
        DataDeckFlavorService.Clear();
        CrosshairHudVisibilityService.Reset();
        PlayerHudStateService.Reset();
        DeckStatusPanel.ReleaseSharedAssets();

        if (activeInstance == this)
        {
            activeInstance = null;
        }
    }

    public override void OnSceneWasLoaded(int buildIndex, string sceneName)
    {
        DataDeckFlavorService.Clear();
        PlayerHudStateService.Reset();
        hudHost?.OnSceneWasLoaded(buildIndex, sceneName);
    }

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
