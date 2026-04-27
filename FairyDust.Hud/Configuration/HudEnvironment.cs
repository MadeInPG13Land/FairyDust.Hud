using System.Reflection;
using MelonLoader;

namespace FairyDust.Hud.Configuration;

public static class HudEnvironment
{
    public static string BaseDirectory => AppContext.BaseDirectory;

    public static string GameRootDirectory => BaseDirectory;

    /// <summary>Folder containing this mod’s DLL (…/Mods) — always writable and easy to find.</summary>
    public static string ModAssemblyDirectory => _modDir ??=
        Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;

    private static string _modDir;

    public static string ModsDirectory => Path.Combine(BaseDirectory, "Mods");

    private static string _userDataDir;

    /// <summary>Same <c>UserData</c> as Melon prefs: read from <c>MelonUtils</c> at runtime (avoids obsolete compile errors on Melon 0.6).</summary>
    public static string UserDataDirectory => _userDataDir ??= ResolveMelonUserDataDirectory();

    private static string ResolveMelonUserDataDirectory()
    {
        // Prefer MelonUtils.UserDataDirectory if present; else GameDirectory + \UserData; else host fallback.
        try
        {
            var utils = typeof(MelonMod).Assembly.GetType("MelonLoader.MelonUtils");
            if (utils != null)
            {
                string ud = GetStaticString(utils, "UserDataDirectory");
                if (!string.IsNullOrEmpty(ud))
                {
                    return ud;
                }

                string game = GetStaticString(utils, "GameDirectory");
                if (!string.IsNullOrEmpty(game))
                {
                    return Path.Combine(game, "UserData");
                }
            }
        }
        catch
        {
        }

        return Path.Combine(BaseDirectory, "UserData");
    }

    private static string GetStaticString(Type t, string name) =>
        t.GetProperty(name, BindingFlags.Public | BindingFlags.Static)?.GetValue(null) as string;

    public static string MelonLoaderDirectory => Path.Combine(BaseDirectory, "MelonLoader");

    public static string Il2CppAssembliesDirectory => Path.Combine(MelonLoaderDirectory, "Il2CppAssemblies");

    public static string ConfigFilePath => Path.Combine(UserDataDirectory, $"{FairyDust.Hud.Metadata.Name}.cfg");

    public static void EnsureUserDataDirectory()
    {
        Directory.CreateDirectory(UserDataDirectory);
    }
}
