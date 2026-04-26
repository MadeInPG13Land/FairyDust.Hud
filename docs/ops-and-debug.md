# Operations: paths, build, Data Deck log

## `UserData` (Melon)

At runtime, **`HudEnvironment.UserDataDirectory`** resolves via **`MelonLoader.MelonUtils`** (reflection) to the game’s **`UserData`** folder — typically:

`C:\Program Files (x86)\Steam\steamapps\common\Forsaken Frontiers\UserData\`

Falls back to `AppContext.BaseDirectory\UserData` if reflection fails.

## Config

- **MelonPreferences** file: `UserData\FairyDust.Hud.cfg` (from `Config.FilePath`).
- Toggles: `Enabled`, per-module flags (`StaminaModuleEnabled`, etc.) — see `Configuration/ModConfiguration.cs`.

## Build and deploy

- Solution: `FairyDust.Hud.sln` (project under `FairyDust.Hud\`).
- **Post-build**: copies `FairyDust.Hud.dll` to **`(GameRoot)\Mods\FairyDust.Hud.dll`** (see `FairyDust.Hud.csproj` target `CopyToGameModsFolder`, runs **after** `Build`).
- If the game is running, the DLL may be locked — close the game before building, or copy `FairyDust.Hud\bin\Debug\net6.0\FairyDust.Hud.dll` into `Mods` by hand.

`Directory.Build.props` (parent folder) sets default **`GameRootDir`**, `MelonLoaderNet6Dir`, `Il2CppAssembliesDir`. Override with **`Local.Build.props`** (from `Local.Build.props.example`) for non-Steam paths.

## Data Deck / CRT **optional** log

These files are for **debugging** the vanilla post stack, not for players.

| File | When |
|------|------|
| `UserData\FairyDust.Stamina.DataDeckPostProcess.log` | Appends a one-line **boot** marker; full content **after you press F10 in-game** |
| `(Mods next to DLL)\FairyDust.Hud_deckpost_last.log` | **Last** F10 dump (overwrite) |
| `(Mods)\FairyDust.Hud_WHERE_LOGS.txt` | Explains paths + F10 (written at startup) |

**F10** runs only while the game is focused; it is **not** required for the stamina overlay.

## Melon log

On init, the mod logs explicit **Warning** lines that the full deck/post dump is **manual** and requires **F10**, plus the exact **UserData** log path.

## Sample probe excerpt (illustrative)

A captured log showed **RLPro\*** effect refs on `FFPlayer`, `DataDeckOpen: True`, deck canvas **World Space**, **worldCamera** = `.../datadeck camera`, and all scene cameras with depth / mask. Use new captures when the game updates.
