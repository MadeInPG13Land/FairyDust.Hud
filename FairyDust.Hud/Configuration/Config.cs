using MelonLoader;

namespace FairyDust.Hud.Configuration;

public static class Config
{
    private static MelonLoader.Preferences.MelonPreferences_ReflectiveCategory category;

    public static string FilePath { get; private set; } = string.Empty;

    public static ModConfiguration Values { get; private set; } = new();
    private static DateTime lastLoadedWriteTimeUtc = DateTime.MinValue;

    public static void Initialize()
    {
        HudEnvironment.EnsureUserDataDirectory();

        FilePath = HudEnvironment.ConfigFilePath;
        bool fileExists = File.Exists(FilePath);

        category = MelonPreferences.CreateCategory<ModConfiguration>(Metadata.Name);
        category.SetFilePath(FilePath, autoload: false, printmsg: false);
        category.LoadFromFile(false);
        category.DestroyFileWatcher();

        Values = category.GetValue<ModConfiguration>() ?? new ModConfiguration();
        ApplyFileOverrides();
        lastLoadedWriteTimeUtc = GetConfigWriteTimeUtc();

        if (!fileExists)
        {
            Save();
            lastLoadedWriteTimeUtc = GetConfigWriteTimeUtc();
        }
    }

    public static void Save()
    {
        GetCategory().SaveToFile(false);
        lastLoadedWriteTimeUtc = GetConfigWriteTimeUtc();
    }

    public static bool ReloadIfChanged()
    {
        DateTime writeTimeUtc = GetConfigWriteTimeUtc();
        if (writeTimeUtc == DateTime.MinValue || writeTimeUtc <= lastLoadedWriteTimeUtc)
        {
            return false;
        }

        ApplyFileOverrides();
        lastLoadedWriteTimeUtc = writeTimeUtc;
        return true;
    }

    public static string LoadedValuesSummary =>
        $"Enabled={Values.Enabled}, "
        + $"Stamina={Values.StaminaModuleEnabled}, "
        + $"BleedOut={Values.BleedOutModuleEnabled}, "
        + $"Infection={Values.InfectionModuleEnabled}, "
        + $"Frostbite={Values.FrostbiteModuleEnabled}";

    private static void ApplyFileOverrides()
    {
        if (!File.Exists(FilePath))
        {
            return;
        }

        Dictionary<string, string> entries = ReadSection(FilePath, Metadata.Name);
        ApplyBool(entries, nameof(ModConfiguration.Enabled), value => Values.Enabled = value);
        ApplyBool(entries, nameof(ModConfiguration.StaminaModuleEnabled), value => Values.StaminaModuleEnabled = value);
        ApplyBool(entries, nameof(ModConfiguration.BleedOutModuleEnabled), value => Values.BleedOutModuleEnabled = value);
        ApplyBool(entries, nameof(ModConfiguration.InfectionModuleEnabled), value => Values.InfectionModuleEnabled = value);
        ApplyBool(entries, nameof(ModConfiguration.FrostbiteModuleEnabled), value => Values.FrostbiteModuleEnabled = value);
        ApplyBool(entries, nameof(ModConfiguration.HudPostProcessingEnabled), value => Values.HudPostProcessingEnabled = value);
    }

    private static Dictionary<string, string> ReadSection(string path, string sectionName)
    {
        var entries = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        bool inSection = false;

        foreach (string rawLine in File.ReadLines(path))
        {
            string line = rawLine.Trim();
            if (line.Length == 0 || line.StartsWith("#") || line.StartsWith(";"))
            {
                continue;
            }

            if (line.StartsWith("[") && line.EndsWith("]"))
            {
                string currentSection = line.Substring(1, line.Length - 2).Trim();
                inSection = string.Equals(currentSection, sectionName, StringComparison.OrdinalIgnoreCase);
                continue;
            }

            if (!inSection)
            {
                continue;
            }

            int separatorIndex = line.IndexOf('=');
            if (separatorIndex <= 0)
            {
                continue;
            }

            string key = line.Substring(0, separatorIndex).Trim();
            string value = line.Substring(separatorIndex + 1).Trim();
            entries[key] = value;
        }

        return entries;
    }

    private static void ApplyBool(Dictionary<string, string> entries, string key, Action<bool> apply)
    {
        if (entries.TryGetValue(key, out string rawValue) && bool.TryParse(rawValue, out bool value))
        {
            apply(value);
        }
    }

    private static DateTime GetConfigWriteTimeUtc()
    {
        try
        {
            return File.Exists(FilePath) ? File.GetLastWriteTimeUtc(FilePath) : DateTime.MinValue;
        }
        catch
        {
            return DateTime.MinValue;
        }
    }

    private static MelonLoader.Preferences.MelonPreferences_ReflectiveCategory GetCategory()
    {
        return category ?? throw new InvalidOperationException("Config.Initialize() must be called before saving preferences.");
    }
}
