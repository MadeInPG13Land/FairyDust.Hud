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

## Component map

| Piece | Role |
|-------|------|
| `DeckStatusPanel` | Reusable multi-row status board for Stamina/Bleed/Infection-style rows. |
| `DeckTextStyle` | Copies live Data Deck TMP font/material/spacing onto HUD labels. |
| `DeckTint` | Applies resolved deck flavor colors to images/text. |
| `StatusBoardModule` | Owns board lifecycle and updates the shared panel. |
| Status submodules | `Stamina`, `Bleed`, `Infection`, and `Frostbite` each produce one optional row. |
| `Experimental/RLPro` | Quarantined render-texture experiment, not part of the normal runtime path. |

## Rebuild rules

`StatusBoardModule` rebuilds the board when the `FFDataDeck` instance changes, so reconnecting or a new session refreshes tint and text style data.
