using System;
using System.IO;
using System.Text;
using FairyDust.Hud.Configuration;

namespace FairyDust.Hud.Debug;

/// <summary>Writes a one-time text file next to the mod DLL so the exact UserData / log paths are obvious.</summary>
internal static class ModLogPaths
{
    private const string ProbeFileName = "FairyDust.Hud_WHERE_LOGS.txt";

    public static void WriteStartupProbe()
    {
        TouchDataDeckUserDataLog();

        try
        {
            string modDir = HudEnvironment.ModAssemblyDirectory;
            if (string.IsNullOrEmpty(modDir) || !Directory.Exists(modDir))
            {
                MelonLoader.MelonLogger.Warning(
                    "[FairyDust.Hud] ModAssemblyDirectory missing; cannot write path probe next to DLL.");
                return;
            }

            var sb = new StringBuilder(512);
            sb.AppendLine("FairyDust.Hud log paths");
            sb.AppendLine("F10 / Ctrl+Shift+D: append Data Deck / post info");
            sb.AppendLine("  " + Path.Combine(HudEnvironment.UserDataDirectory, "FairyDust.Stamina.DataDeckPostProcess.log"));
            sb.AppendLine("  mirror: " + Path.Combine(modDir, "FairyDust.Hud_deckpost_last.log"));
            sb.AppendLine("UserData: " + HudEnvironment.UserDataDirectory);
            string wherePath = Path.Combine(modDir, ProbeFileName);
            File.WriteAllText(wherePath, sb.ToString(), Encoding.UTF8);
            MelonLoader.MelonLogger.Msg("[FairyDust.Hud] Wrote " + wherePath);
        }
        catch (Exception ex)
        {
            MelonLoader.MelonLogger.Warning("[FairyDust.Hud] Path probe: " + ex.Message);
        }
    }

    /// <summary>Creates/updates the UserData append log on boot so the path is not “invisible” until F10.</summary>
    private static void TouchDataDeckUserDataLog()
    {
        const string name = "FairyDust.Stamina.DataDeckPostProcess.log";
        string path = Path.Combine(HudEnvironment.UserDataDirectory, name);
        try
        {
            HudEnvironment.EnsureUserDataDirectory();
            string line = "---\n# FairyDust.Hud @ " + DateTime.UtcNow.ToString("o")
                          + " — mod loaded. F10 appends the Data Deck / post stack dump to this file.\n";
            File.AppendAllText(path, line, Encoding.UTF8);
            MelonLoader.MelonLogger.Msg("[FairyDust.Hud] Touched (append) UserData file: " + path);
        }
        catch (Exception ex)
        {
            MelonLoader.MelonLogger.Error(
                "[FairyDust.Hud] Could not write UserData log at `" + path + "`: " + ex.Message);
        }
    }

}
