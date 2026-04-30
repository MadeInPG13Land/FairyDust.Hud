# Operations: paths, build, runtime notes

## UserData

At runtime, `HudEnvironment.UserDataDirectory` resolves through MelonLoader to the game's `UserData` folder, typically:

`C:\Program Files (x86)\Steam\steamapps\common\Forsaken Frontiers\UserData\`

It falls back to `AppContext.BaseDirectory\UserData` if MelonLoader path reflection fails.

## Config

- MelonPreferences file: `UserData\FairyDust.Hud.cfg`.
- Toggles: `Enabled`, `StaminaModuleEnabled`, `BleedOutModuleEnabled`, `InfectionModuleEnabled`, `FrostbiteModuleEnabled`.
- Config is for player-facing switches only; layout and visual tuning live in code.

## Build and deploy

- Solution: `FairyDust.Hud.sln`.
- Post-build copy: `FairyDust.Hud.dll` deploys to `(GameRoot)\Mods\FairyDust.Hud.dll`.
- If deployment fails, close the game first; the DLL may be locked by MelonLoader.

`Directory.Build.props` sets default `GameRootDir`, `MelonLoaderNet6Dir`, and `Il2CppAssembliesDir`. Use `Local.Build.props` for local path overrides.

## Runtime notes

- The HUD is created once by `GameplayHudHost` and hidden outside active gameplay scenes.
- The normal runtime path is a Screen Space Overlay canvas.
- The HUD is hidden while the Data Deck is open, and while the local player's Data Deck Crosshair setting is off.
- The Bleed row only represents a lethal bleed-out countdown; Exploration mode and non-countdown bleeding states are hidden.
- RLPro/render-texture work is quarantined under `Experimental/RLPro` and is not wired into runtime.

## Future mod menu

A shared in-game FairyDust mod menu should be a separate mod-level project, such as `FairyDust.ModMenu`, with a small registration API that other FairyDust mods can hook into. `FairyDust.Hud` should not grow that framework directly.
