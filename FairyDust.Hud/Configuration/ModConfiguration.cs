namespace FairyDust.Hud.Configuration;

/// <summary>MelonPreferences-backed options players are meant to change. Not for layout or gameplay tuning.</summary>
public sealed class ModConfiguration
{
    public bool Enabled = true;

    public bool StaminaModuleEnabled = true;
    public bool BleedOutModuleEnabled = true;
    public bool InfectionModuleEnabled = true;
    public bool FrostbiteModuleEnabled = true;
}
