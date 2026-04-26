using FairyDust.Template.Configuration;
using MelonLoader;

namespace FairyDust.Template;

public sealed class Main : MelonMod
{
    public override void OnInitializeMelon()
    {
        Config.Initialize();

        LoggerInstance.Msg($"{Metadata.Name} v{Metadata.Version} initialising.");
        LoggerInstance.Msg($"Game root: {FairyDustEnvironment.GameRootDirectory}");
        LoggerInstance.Msg($"UserData: {FairyDustEnvironment.UserDataDirectory}");
        LoggerInstance.Msg($"Il2Cpp assemblies: {FairyDustEnvironment.Il2CppAssembliesDirectory}");
        LoggerInstance.Msg($"Preferences file: {Config.FilePath}");
    }
}
