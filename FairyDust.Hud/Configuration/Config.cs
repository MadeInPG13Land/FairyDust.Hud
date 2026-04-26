using MelonLoader;

namespace FairyDust.Hud.Configuration;

public static class Config
{
    private static MelonLoader.Preferences.MelonPreferences_ReflectiveCategory category;

    public static string FilePath { get; private set; } = string.Empty;

    public static ModConfiguration Values { get; private set; } = new();

    public static void Initialize()
    {
        HudEnvironment.EnsureUserDataDirectory();

        FilePath = HudEnvironment.ConfigFilePath;

        category = MelonPreferences.CreateCategory<ModConfiguration>(Metadata.Name);
        category.SetFilePath(FilePath, printmsg: false);
        category.DestroyFileWatcher();

        Values = category.GetValue<ModConfiguration>();

        if (!File.Exists(FilePath))
        {
            Save();
        }
    }

    public static void Save()
    {
        GetCategory().SaveToFile(false);
    }

    private static MelonLoader.Preferences.MelonPreferences_ReflectiveCategory GetCategory()
    {
        return category ?? throw new InvalidOperationException("Config.Initialize() must be called before saving preferences.");
    }
}
