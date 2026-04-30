# Mod: how FairyDust.Hud uses the Data Deck

## Local player

- `FairyLocalPlayer.Current` resolves `FairyEngine.LocalPlayer.TryCast<FFPlayer>()`.
- No `FFDataDeck` is available without a valid `FFPlayer` in the active Forsaken Frontiers scene.

## HUD rendering

| | FairyDust HUD | Vanilla Data Deck |
|--|---------------|-------------------|
| Canvas | Built by `GameplayHudHost`, docked bottom-left | `FFDataDeck._canvas`, world-space PDA UI |
| Render path | Screen Space Overlay canvas | Dedicated Data Deck camera under the player camera rig |
| Purpose | Compact gameplay status board | Full PDA UI in front of the character |

Custom bars are not parented under `FFDataDeck._canvas`. They match deck styling by copying TMP style and sampling the live deck flavor color.

The HUD also follows the local Data Deck Crosshair setting as the first-pass user-facing visibility toggle. Crosshair off means the whole FairyDust HUD is hidden until Crosshair is enabled again.

## Component map

| Piece | Role |
|-------|------|
| `DeckStatusPanel` | Reusable multi-row status board for Stamina/Bleed/Infection-style rows. |
| `DeckTextStyle` | Copies live Data Deck TMP font/material/spacing onto HUD labels. |
| `DeckTint` | Applies resolved deck flavor colors to images/text. |
| `StatusBoardModule` | Owns board lifecycle and updates the shared panel. |
| Status submodules | `Stamina`, `Bleed`, `Infection`, and `Frostbite` each produce one optional row. |
| Crosshair visibility patches | Track the Data Deck Crosshair toggle and hide the shared status board when it is disabled. |
| `Experimental/RLPro` | Quarantined render-texture experiment, not part of the normal runtime path. |

See [modular-structure.md](modular-structure.md) for the module/service/entity boundaries used by status submodules.

## Rebuild rules

`StatusBoardModule` rebuilds the board when the `FFDataDeck` instance changes, so reconnecting or a new session refreshes tint and text style data.
