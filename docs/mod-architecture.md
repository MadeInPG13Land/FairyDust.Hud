# Mod: how FairyDust.Hud uses the Data Deck

## Local player

- **`FairyLocalPlayer.Current`** → `FairyEngine.LocalPlayer.TryCast<FFPlayer>()` (`Modules/Hud/Interop/FairyLocalPlayer.cs`).
- No `FFDataDeck` without a valid **`FFPlayer`** in the active **Forsaken Frontiers** scene (see `WorldSceneGate`).

## Screen overlay (this mod) vs world deck (vanilla)

| | **FairyDust HUD** | **Vanilla Data Deck** |
|--|-------------------|------------------------|
| Canvas | **`GameplayHudHost`**: `ScreenSpaceOverlay`, `DontDestroyOnLoad`, bottom-left dock | `FFDataDeck._canvas`: **World Space** + **datadeck camera** |
| Purpose | Stamina / optional modules in a corner | Full PDA UI in front of the character |

Custom bars are **not** parented under `FFDataDeck._canvas`; they **match** deck styling by using the same **tint** components, **TMP** font copy, and **line** aesthetics where possible.

## Component map (Data Deck–related)

| Piece | Role |
|-------|------|
| **`DeckTint`** | Adds **`FFDataDeckUIColorer`** to `Image` / `TextMeshProUGUI` so colors track deck **flavor**. |
| **`LineFrame`** | Thin white frame + optional vertical bar; **`DeckTint.BindImage`** when `FFDataDeck` is non-null. |
| **`FillBar.Slim`** | Track + horizontal fill; color updated by gameplay (e.g. stamina white / red zone). |
| **`StaminaHudPanel`** | Builds framed row: icon (bleed sprite from deck), divider, TMP “STAMINA”, bar. Uses deck for **style** + **bleed** art. |
| **`StaminaHudBleedIcon`** | Finds **`Icon_Bleeding`** / **`bleeding_fill`** (or similar) under the deck hierarchy; caches sprites per deck instance. |

## Rebuild rules

`StaminaHudPanel` rebuilds when **slot parent**, **`FFDataDeck` instance identity**, or panel integrity changes — so reconnecting or a new session re-binds tint and icons.

## Optional: other building blocks

- **`DeckPanel`** (`Components/Deck/DeckPanel.cs`) — alternate **Data Deck–style** row layout (chrome metrics, stroke width). Not required for the minimal stamina path.

## Debug probe (optional)

- **`DataDeckPostProcessLog`**: appends a technical report (RLPro fields on `FFPlayer`, `FFDataDeck._canvas` metadata, cameras). Triggered in-game by **F10** (see [ops-and-debug.md](ops-and-debug.md)); not part of normal HUD behavior.
