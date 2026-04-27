# Operations: paths, build, runtime notes

## UserData

At runtime, `HudEnvironment.UserDataDirectory` resolves through MelonLoader to the game's `UserData` folder, typically:

`C:\Program Files (x86)\Steam\steamapps\common\Forsaken Frontiers\UserData\`

It falls back to `AppContext.BaseDirectory\UserData` if MelonLoader path reflection fails.

## Config

- MelonPreferences file: `UserData\FairyDust.Hud.cfg`.
- Toggles: `Enabled`, `StaminaModuleEnabled`, `BleedOutModuleEnabled`, `InfectionModuleEnabled`.
- Config is for player-facing switches only; layout and visual tuning live in code.

## Build and deploy

- Solution: `FairyDust.Hud.sln`.
- Post-build copy: `FairyDust.Hud.dll` deploys to `(GameRoot)\Mods\FairyDust.Hud.dll`.
- If deployment fails, close the game first; the DLL may be locked by MelonLoader.

`Directory.Build.props` sets default `GameRootDir`, `MelonLoaderNet6Dir`, and `Il2CppAssembliesDir`. Use `Local.Build.props` for local path overrides.

## Runtime notes

- The HUD is created once by `GameplayHudHost` and hidden outside active gameplay scenes.
- `HudPostProcessRig` is experimental: it renders the HUD through a dedicated camera and RLPro profile before displaying it as overlay UI.
- If the RLPro path renders a black rectangle or invisible HUD in-game, inspect camera clear flags, render texture alpha, and the post-process profile first.
