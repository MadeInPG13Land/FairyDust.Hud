using System.IO;
using FairyDust.Hud.Configuration;
using FairyDust.Hud.Debug;
using FairyDust.Hud.Modules.BleedOut;
using FairyDust.Hud.Modules.Hud;
using FairyDust.Hud.Modules.Infection;
using FairyDust.Hud.Modules.Stamina;
using MelonLoader;
using UnityEngine;

namespace FairyDust.Hud;

public sealed class Main : MelonMod
{
    private const string PostLogFileName = "FairyDust.Stamina.DataDeckPostProcess.log";
    private static int _lastPostLogFrame = -1;

    private GameplayHudHost hudHost;

    public override void OnInitializeMelon()
    {
        Config.Initialize();

        hudHost = new GameplayHudHost(this);
        hudHost.Register(new StaminaHudModule());
        hudHost.Register(new BleedOutHudModule());
        hudHost.Register(new InfectionHudModule());
        hudHost.Initialize();

        LoggerInstance.Msg($"{Metadata.Name} v{Metadata.Version} — {Config.FilePath}");
        string postLog = Path.Combine(HudEnvironment.UserDataDirectory, PostLogFileName);
        ModLogPaths.WriteStartupProbe();
        LoggerInstance.Msg("Dev: F10 / Ctrl+Shift+D appends " + postLog);
    }

    public override void OnUpdate()
    {
        if (UnityEngine.Input.GetKeyDown(KeyCode.F10))
        {
            if (Time.frameCount == _lastPostLogFrame)
            {
                return;
            }

            _lastPostLogFrame = Time.frameCount;
            DataDeckPostProcessLog.TryDump();
        }

    }

    /// <summary>Catch chord when legacy <see cref="Input" /> is disabled (new Input System).</summary>
    public override void OnGUI()
    {
        Event e = Event.current;
        if (e == null || e.type != EventType.KeyDown)
        {
            return;
        }

        if (e.control && e.shift && e.keyCode == KeyCode.D)
        {
            if (Time.frameCount == _lastPostLogFrame)
            {
                return;
            }

            _lastPostLogFrame = Time.frameCount;
            DataDeckPostProcessLog.TryDump();
            return;
        }

    }

    public override void OnSceneWasLoaded(int buildIndex, string sceneName) =>
        hudHost?.OnSceneWasLoaded(buildIndex, sceneName);

    public override void OnLateUpdate() =>
        hudHost?.OnLateUpdate();
}
