# FairyDust.Hud

MelonLoader mod for **Forsaken Frontiers** (IL2CPP): gameplay HUD with a stamina row first and more widgets later.

Repo folder may still be named `FairyDust.Stamina` on disk; the solution and assembly are **FairyDust.Hud**.

## Layout

- `FairyDust.Hud/` - mod project
- `FairyDust.Hud/Configuration/` - MelonPreferences config and `HudEnvironment` paths
- `FairyDust.Hud/Components/Deck/` - reusable Data Deck-style HUD components
- `FairyDust.Hud/Modules/Hud/` - shared HUD shell, dock, scene gate, local player lookup, and RLPro render rig
- `FairyDust.Hud/Modules/Status/` - compact stamina / bleed / infection status board

Registration order in `Main` is bottom to top on screen; the first module sits nearest the bottom margin.

## Entry and config

- `Main.cs` - MelonLoader bootstrap
- `Config.cs` / `ModConfiguration.cs` - player-facing toggles only. File: `UserData/FairyDust.Hud.cfg`
- `DeckStatusPanel.cs` - reusable multi-row status board component
- `HudDockLayout.cs` - shared HUD dock metrics, not player preferences

## Local build

Default game root:

`C:\Program Files (x86)\Steam\steamapps\common\Forsaken Frontiers`

Override with `Local.Build.props` copied from `Local.Build.props.example`.

```powershell
dotnet build FairyDust.Hud.sln
dotnet format FairyDust.Hud.sln
```

Successful build copies `FairyDust.Hud.dll` to the game `Mods` folder. Remove any old `FairyDust.Stamina.dll` so MelonLoader does not load both.

## MelonLoader

- `MelonGame("made in fairyland", "Forsaken Frontiers")`
- `MelonProcess("Forsaken Frontiers.exe")`
- `MelonPlatformDomain(IL2CPP)`
