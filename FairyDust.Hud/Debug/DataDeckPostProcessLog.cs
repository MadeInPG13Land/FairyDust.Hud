using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using FairyDust.Hud.Configuration;
using FairyDust.Hud.Modules.Hud;
using FairyDust.Hud.Modules.Hud.Interop;
using Il2Cppmadeinfairyland.forsakenfrontiers.actor.player;
using Il2Cppmadeinfairyland.forsakenfrontiers.actor.player.datadeck;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace FairyDust.Hud.Debug;

/// <summary>
/// Dumps file log for Data Deck CRT-style stack (RetroLook RLPro* on Post Process v2). Invoked from
/// <see cref="Main.OnGUI"/> (Ctrl+Shift+D). Uses IMGUI key path so it still works when legacy <c>Input</c> is inactive.
/// </summary>
internal static class DataDeckPostProcessLog
{
    private const string FileName = "FairyDust.Stamina.DataDeckPostProcess.log";

    /// <summary>Append one report. Always writes a file when possible (no strict scene block).</summary>
    public static void TryDump()
    {
        MelonLoader.MelonLogger.Msg("[FairyDust.Hud] Data deck post log: dumping…");
        FFPlayer player = null;
        try
        {
            player = FairyLocalPlayer.Current;
        }
        catch (Exception ex)
        {
            MelonLoader.MelonLogger.Warning($"[FairyDust.Hud] Local player: {ex.Message}");
        }

        FFDataDeck deck = null;
        try
        {
            deck = player?.DataDeck;
        }
        catch (Exception ex)
        {
            MelonLoader.MelonLogger.Warning($"[FairyDust.Hud] DataDeck: {ex.Message}");
        }

        try
        {
            WriteLog(player, deck);
        }
        catch (Exception ex)
        {
            MelonLoader.MelonLogger.Warning($"[FairyDust.Hud] Data deck post log failed: {ex}");
        }
    }

    private static void WriteLog(FFPlayer player, FFDataDeck deck)
    {
        HudEnvironment.EnsureUserDataDirectory();
        string path = Path.Combine(HudEnvironment.UserDataDirectory, FileName);
        var sb = new StringBuilder(8192);
        DateTime now = DateTime.UtcNow;
        Scene s = SceneManager.GetActiveScene();
        sb.AppendLine("---");
        sb.AppendLine($"## Data deck post / CRT stack probe @ {now:yyyy-MM-dd HH:mm:ss}Z");
        sb.AppendLine($"Active scene: `{s.name}` (loaded={s.isLoaded})");
        sb.AppendLine(
            $"World gate: activeSceneMatch={WorldSceneGate.IsActiveGameplayScene} lastLoadedMatch={WorldSceneGate.IsForsakenWorldLoaded}");
        sb.AppendLine();
        sb.AppendLine("Summary: **RetroLook Pro** (`RLPro*` in game DLL) on Unity **Post Process Stack v2**; `FFPlayer` holds");
        sb.AppendLine("RLPro* effect settings. Scanlines, smear, vignette, and TV resampling are from that stack, not Canvas/TMP alone.");
        sb.AppendLine();

        sb.AppendLine("### FFPlayer RLPro* (via reflection; avoids PostProcess stack assembly ref)");
        AppendRlFromPlayerByReflection(sb, player);
        sb.AppendLine();

        sb.AppendLine("### FFDataDeck");
        if (deck == null)
        {
            sb.AppendLine("DataDeck: null (not loaded or no local player).");
        }
        else
        {
            sb.AppendLine($"DataDeckOpen: {deck.DataDeckOpen}");
        }

        sb.AppendLine();
        sb.AppendLine("### FFDataDeck._canvas");
        Canvas canvas = deck != null ? deck._canvas : null;
        if (canvas == null)
        {
            sb.AppendLine("_canvas: null");
        }
        else
        {
            sb.AppendLine($"renderMode: {canvas.renderMode}");
            sb.AppendLine($"sortingOrder: {canvas.sortingOrder}");
            Camera wc = canvas.worldCamera;
            sb.AppendLine($"worldCamera: {(wc == null ? "null" : GetTransformPath(wc.transform))}");
        }

        sb.AppendLine();
        sb.AppendLine("### Components on deck-canvas tree (PDA / datadeck / DataDeck in path) — post / RL / camera");
        if (canvas != null)
        {
            var comps = canvas.GetComponentsInChildren<Component>(true);
            for (int i = 0; i < comps.Length; i++)
            {
                var comp = comps[i];
                if (comp is Transform)
                {
                    continue;
                }

                if (comp == null)
                {
                    continue;
                }

                string p = GetTransformPath(comp.transform);
                if (!PathLooksDeckRelated(p))
                {
                    continue;
                }

                string cn = comp.GetType().Name;
                if (comp is Camera cam)
                {
                    sb.AppendLine(
                        $"- {p} :: Camera d={cam.depth} mask={cam.cullingMask}");
                    continue;
                }

                if (!IsPostRelatedTypeName(cn))
                {
                    continue;
                }

                if (comp is MonoBehaviour m)
                {
                    sb.AppendLine($"- {p} :: {comp.GetType().FullName} enabled={m.enabled}");
                }
                else
                {
                    sb.AppendLine($"- {p} :: {comp.GetType().FullName}");
                }
            }
        }

        sb.AppendLine();
        sb.AppendLine("### Active UnityEngine.Cameras (all)");
        {
            var cams = Object.FindObjectsOfType<Camera>(true);
            for (int i = 0; i < cams.Length; i++)
            {
                Camera cam = cams[i];
                if (cam == null)
                {
                    continue;
                }

                sb.AppendLine(
                    $"- d={cam.depth} mask={cam.cullingMask} name={cam.name} path={GetTransformPath(cam.transform)}");
            }
        }

        string text = sb.ToString();
        File.AppendAllText(path, text, Encoding.UTF8);
        string modDir = HudEnvironment.ModAssemblyDirectory;
        if (!string.IsNullOrEmpty(modDir))
        {
            string mirror = Path.Combine(modDir, "FairyDust.Hud_deckpost_last.log");
            File.WriteAllText(mirror, text, Encoding.UTF8);
        }

        bool ok = File.Exists(path);
        long len = ok ? new FileInfo(path).Length : 0;
        MelonLoader.MelonLogger.Msg(
            $"[FairyDust.Hud] Data deck post log: exists={ok} bytes={len} UserData file: {path}");
        if (!string.IsNullOrEmpty(modDir))
        {
            MelonLoader.MelonLogger.Msg(
                "[FairyDust.Hud] …and mirror next to mod: " + Path.Combine(modDir, "FairyDust.Hud_deckpost_last.log"));
        }
    }

    private static bool PathLooksDeckRelated(string p) =>
        p.Contains("pda", StringComparison.OrdinalIgnoreCase)
        || p.Contains("datadeck", StringComparison.OrdinalIgnoreCase)
        || p.Contains("DataDeck", StringComparison.OrdinalIgnoreCase);

    private static readonly string[] RlPropertyNames =
    {
        "_pulsatingVignette", "_glitch3", "_crtAperture", "_bleed", "_phosphor", "_vignette",
    };

    private static void AppendRlFromPlayerByReflection(StringBuilder sb, FFPlayer player)
    {
        if (player == null)
        {
            sb.AppendLine("(no local FFPlayer — load into Forsaken world so RLPro lines populate.)");
            return;
        }

        Type t = typeof(FFPlayer);
        for (int i = 0; i < RlPropertyNames.Length; i++)
        {
            string n = RlPropertyNames[i];
            PropertyInfo prop = t.GetProperty(n, BindingFlags.Instance | BindingFlags.Public);
            if (prop == null)
            {
                sb.AppendLine($"{n}: (property missing)");
                continue;
            }

            object v;
            try
            {
                v = prop.GetValue(player);
            }
            catch (Exception ex)
            {
                sb.AppendLine($"{n}: read error: {ex.Message}");
                continue;
            }

            if (v == null)
            {
                sb.AppendLine($"{n}: null");
                continue;
            }

            if (v is Object uo)
            {
                sb.AppendLine(
                    $"{n}: {v.GetType().FullName} name=\"{uo.name}\" id={uo.GetInstanceID()}");
            }
            else
            {
                sb.AppendLine($"{n}: {v.GetType().FullName} toString={v}");
            }
        }
    }

    private static bool IsPostRelatedTypeName(string tn) =>
        tn.Contains("RLPro", StringComparison.OrdinalIgnoreCase)
        || tn.Contains("PostProcess", StringComparison.OrdinalIgnoreCase)
        || (tn.Contains("Post", StringComparison.OrdinalIgnoreCase) && tn.Contains("Process", StringComparison.OrdinalIgnoreCase));

    private static string GetTransformPath(Transform t)
    {
        if (t == null)
        {
            return string.Empty;
        }

        var stack = new Stack<string>();
        Transform cur = t;
        int guard = 0;
        while (cur != null && guard++ < 200)
        {
            stack.Push(cur.name);
            cur = cur.parent;
        }

        return string.Join("/", stack);
    }
}
