using System.IO;

namespace FairyDust.Template.Configuration;

public static class FairyDustEnvironment
{
    public static string BaseDirectory => AppContext.BaseDirectory;

    public static string GameRootDirectory => BaseDirectory;

    public static string ModsDirectory => Path.Combine(BaseDirectory, "Mods");

    public static string UserDataDirectory => Path.Combine(BaseDirectory, "UserData");

    public static string MelonLoaderDirectory => Path.Combine(BaseDirectory, "MelonLoader");

    public static string Il2CppAssembliesDirectory => Path.Combine(MelonLoaderDirectory, "Il2CppAssemblies");

    public static string ConfigFilePath => Path.Combine(UserDataDirectory, $"{Metadata.Name}.cfg");

    public static void EnsureUserDataDirectory()
    {
        Directory.CreateDirectory(UserDataDirectory);
    }
}
