# FairyDust.Hud

MelonLoader mod for **Forsaken Frontiers** (IL2CPP): gameplay HUD (stamina row first; more widgets later).

Repo folder may still be named `FairyDust.Stamina` on disk; the solution and assembly are **FairyDust.Hud**.

## Layout

- `FairyDust.Hud/` — mod project
- `FairyDust.Hud/Configuration/` — MelonPreferences config and `HudEnvironment` paths
- `FairyDust.Hud/Modules/Hud/` — shared HUD shell (`GameplayHudHost`, dock layout, `IHudModule`, `WorldSceneGate`, `FairyLocalPlayer`)
- `FairyDust.Hud/Modules/Stamina/` — stamina panel + module
- `FairyDust.Hud/Modules/BleedOut/` — bleed-out placeholder module
- `FairyDust.Hud/Modules/Infection/` — infection placeholder module

Registration order in `Main` is bottom → top on screen (first module sits nearest the bottom margin).

## Entry and config

- `Main.cs` — MelonLoader bootstrap
- `Config.cs` / `ModConfiguration.cs` — **player-facing toggles only** (mod on/off, per-module on/off). File: `UserData/FairyDust.Hud.cfg`
- `Modules/Hud/HudDockLayout.cs`, `Modules/Hud/DeckStylePanelChrome.cs` — shared DataDeck-style panel chrome + layout metrics (not preferences)

## Local build

Default game root: `C:\Program Files (x86)\Steam\steamapps\common\Forsaken Frontiers`

Override with `Local.Build.props` (copy from `Local.Build.props.example`).

```powershell
dotnet build FairyDust.Hud.sln
dotnet format FairyDust.Hud.sln
```

Successful build copies `FairyDust.Hud.dll` to the game `Mods` folder. Remove any old `FairyDust.Stamina.dll` so MelonLoader does not load both.

## MelonLoader

- `MelonGame("made in fairyland", "Forsaken Frontiers")`
- `MelonProcess("Forsaken Frontiers.exe")`
- `MelonPlatformDomain(IL2CPP)`
