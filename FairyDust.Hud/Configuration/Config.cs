using MelonLoader;

namespace FairyDust.Hud.Configuration;

public static class Config
{
    private static MelonLoader.Preferences.MelonPreferences_ReflectiveCategory category;
    private static readonly HashSet<string> LoggedInvalidConfigEntries = new(StringComparer.OrdinalIgnoreCase);

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

        Values = LoadValuesFromFile();
        lastLoadedWriteTimeUtc = GetConfigWriteTimeUtc();

        if (!fileExists)
        {
            Save();
            Values = LoadValuesFromFile();
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

        ModConfiguration reloadedValues = LoadValuesFromFile();
        Values = reloadedValues;
        lastLoadedWriteTimeUtc = writeTimeUtc;
        return true;
    }

    private static ModConfiguration LoadValuesFromFile()
    {
        var values = new ModConfiguration();
        ApplyFileOverrides(values);
        return values;
    }

    private static void ApplyFileOverrides(ModConfiguration values)
    {
        if (!File.Exists(FilePath))
        {
            return;
        }

        Dictionary<string, string> entries = ReadSection(FilePath, Metadata.Name);
        ApplyBool(entries, nameof(ModConfiguration.Enabled), value => values.Enabled = value);
        ApplyBool(entries, nameof(ModConfiguration.StaminaModuleEnabled), value => values.StaminaModuleEnabled = value);
        ApplyBool(entries, nameof(ModConfiguration.BleedOutModuleEnabled), value => values.BleedOutModuleEnabled = value);
        ApplyBool(entries, nameof(ModConfiguration.InfectionModuleEnabled), value => values.InfectionModuleEnabled = value);
        ApplyBool(entries, nameof(ModConfiguration.FrostbiteModuleEnabled), value => values.FrostbiteModuleEnabled = value);
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
        if (!entries.TryGetValue(key, out string rawValue))
        {
            return;
        }

        if (bool.TryParse(rawValue, out bool value))
        {
            apply(value);
            return;
        }

        string warningKey = key + "=" + rawValue;
        if (LoggedInvalidConfigEntries.Add(warningKey))
        {
            MelonLogger.Warning("[FairyDust.Hud] Ignoring invalid bool config value for " + key + ": " + rawValue);
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
