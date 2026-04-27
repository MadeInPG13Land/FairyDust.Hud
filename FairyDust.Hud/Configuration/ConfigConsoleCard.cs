using System.Text;

namespace FairyDust.Hud.Configuration;

internal static class ConfigConsoleCard
{
    private const int Width = 58;
    private static readonly string Border = "+" + new string('-', Width) + "+";

    public static string Build()
    {
        var sb = new StringBuilder();
        sb.AppendLine();
        sb.AppendLine(Border);
        sb.AppendLine(RowCentered(Metadata.Name));
        sb.AppendLine(Border);
        sb.AppendLine(RowKeyValue("Version", Metadata.Version));
        sb.AppendLine(RowKeyValue("File", DisplayConfigPath()));
        sb.AppendLine(Border);
        sb.AppendLine(RowText("Status Modules"));
        sb.AppendLine(RowText(string.Empty));
        sb.AppendLine(RowSetting(Config.Values.Enabled, "HUD"));
        sb.AppendLine(RowSetting(Config.Values.StaminaModuleEnabled, "Stamina"));
        sb.AppendLine(RowSetting(Config.Values.BleedOutModuleEnabled, "Bleed Out"));
        sb.AppendLine(RowSetting(Config.Values.InfectionModuleEnabled, "Infection"));
        sb.AppendLine(RowSetting(Config.Values.FrostbiteModuleEnabled, "Frostbite"));
        sb.AppendLine(Border);
        sb.AppendLine(RowText("Auto-reload is active. Changes apply while in game."));
        sb.Append(Border);
        return sb.ToString();
    }

    private static string DisplayConfigPath()
    {
        try
        {
            string userData = HudEnvironment.UserDataDirectory;
            string path = Config.FilePath;
            if (!string.IsNullOrEmpty(userData)
                && !string.IsNullOrEmpty(path)
                && path.StartsWith(userData, StringComparison.OrdinalIgnoreCase))
            {
                string fileName = Path.GetFileName(path);
                if (!string.IsNullOrEmpty(fileName))
                {
                    return Path.Combine("UserData", fileName);
                }
            }
        }
        catch
        {
        }

        return Config.FilePath;
    }

    private static string RowSetting(bool enabled, string label) =>
        RowText("  " + (enabled ? "[ON]  " : "[OFF] ") + label);

    private static string RowKeyValue(string key, string value) =>
        RowText(key.PadRight(9) + (value ?? string.Empty));

    private static string RowCentered(string value)
    {
        value ??= string.Empty;
        if (value.Length >= Width)
        {
            return RowText(value);
        }

        int left = (Width - value.Length) / 2;
        return RowText(new string(' ', left) + value);
    }

    private static string RowText(string value)
    {
        value ??= string.Empty;
        if (value.Length > 0 && !char.IsWhiteSpace(value[0]))
        {
            value = "  " + value;
        }

        if (value.Length > Width)
        {
            value = value.Substring(0, Width);
        }

        return "|" + value.PadRight(Width) + "|";
    }
}
