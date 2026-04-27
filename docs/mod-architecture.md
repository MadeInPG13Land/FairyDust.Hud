# Mod: how FairyDust.Hud uses the Data Deck

## Local player

- `FairyLocalPlayer.Current` resolves `FairyEngine.LocalPlayer.TryCast<FFPlayer>()`.
- No `FFDataDeck` is available without a valid `FFPlayer` in the active Forsaken Frontiers scene.

## HUD rendering

| | FairyDust HUD | Vanilla Data Deck |
|--|---------------|-------------------|
| Canvas | Built by `GameplayHudHost`, docked bottom-left | `FFDataDeck._canvas`, world-space PDA UI |
| Render path | HUD canvas renders through `HudPostProcessRig` into a texture, then displays as overlay | Dedicated Data Deck camera under the player camera rig |
| Purpose | Stamina / future compact modules | Full PDA UI in front of the character |

Custom bars are not parented under `FFDataDeck._canvas`. They match deck styling by copying TMP style, binding deck tint colorers, and rendering through a small RLPro post-process experiment.

## Component map

| Piece | Role |
|-------|------|
| `DeckStatusPanel` | Reusable multi-row status board for Stamina/Bleed/Infection-style rows. |
| `DeckTextStyle` | Copies live Data Deck TMP font/material/spacing onto HUD labels. |
| `DeckTint` | Adds `FFDataDeckUIColorer` to images/text so colors track deck flavor. |
| `HudPostProcessRig` | Renders HUD UI through a dedicated camera and RLPro/PostProcess profile. |
| `StatusHudModule` | Reads `FFPlayer` stamina, bleeding, and infection state into the board rows. |

## Rebuild rules

`StatusHudModule` rebuilds the board when the `FFDataDeck` instance changes, so reconnecting or a new session rebinds tint and text style data.
