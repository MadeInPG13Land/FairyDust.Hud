# Game: Data Deck (engine)

## Player link

- **`FFPlayer`** (`madeinfairyland.forsakenfrontiers.actor.player`) exposes **`DataDeck`** → **`FFDataDeck`** instance for the local character.
- **`FFDataDeck`** (`...actor.player.datadeck`) is a **FishNet `NetworkBehaviour`**. It references the owning player, deck open state, UI groups, renderers, audio, etc.

## Canvas and camera (from a live probe)

A typical capture shows:

- **`FFDataDeck._canvas`**: **`RenderMode.WorldSpace`**, `sortingOrder: 0`.
- **`worldCamera`**: path ends in **`.../Camera Controller/camera/datadeck camera`** — a dedicated **Camera** for the deck view (depth **100**, culling mask **33554432** in one run; main `camera` may be depth **0** with a broad mask).

So Data Deck UI is **world-space uGUI** rendered by a **child camera** under the player camera rig, not the main gameplay camera’s overlay.

## Text and images (tinting)

- **`FFDataDeckUIColorer`** (`madeinfairyland.forsakenfrontiers.ui`) — component on deck **Image** / **TextMeshProUGUI** fields **`_image`**, **`_text`**. Call **`AssignDataDeck(FFDataDeck)`**, **`ListenForFlavorChanges()`**; **`ApplyColor(FlavorData)`** when flavor/theme changes.
- Built-in deck uses **TMP** with **TextMeshPro/Distance Field** materials (see research dumps under `UserData` / in-game hierarchy `Text (TMP)`).

## CRT / “filter” look (post-processing)

The visible scanlines, chromatic fringing, vignette, and TV-style artifacts are **not** the Unity UI canvas shader alone. The game ships **RetroLook Pro**–style effects as **`RLPro*`** types (namespace **`Il2Cpp`**, **Post Process Stack v2** / `PostProcessEffectSettings`).

**`FFPlayer`** holds direct references to several effect **settings** instances (names from prior runtime research):

| Field | Type (Il2Cpp) |
|-------|----------------|
| `_pulsatingVignette` | `RLProPulsatingVignette` |
| `_glitch3` | `RLProGlitch3` |
| `_crtAperture` | `RLProCRTAperture` |
| `_bleed` | `RLProBleed` |
| `_phosphor` | `RLProPhosphor` |
| `_vignette` | `RLProUltimateVignette` |

Other RL types exist in the same DLL (e.g. TV, VHS, low-res, noise) and may sit on **PostProcessVolume** profiles bound to the deck camera stack.

## Useful `FFDataDeck` fields (non-exhaustive)

- **`_canvas`**, **`DataDeckOpen`**
- **`inventoryUIGroup`**, **`optionsUIGroup`**, category / shop / minigame groups
- **`gameTime`**, **`flavorText`**, **`characterName`**, **`credits`**, etc. — **TMP** references for copying font/style
- **`dataDeckRenderer`**, **`armsRenderer`**, **`_dataDeckRenderers`** — 3D mesh side of the PDA prop

## Hierarchy hint (from probes)

Paths such as:

`FFPlayer_Male/.../Camera Controller/camera/datadeck camera/DataDeck/.../pda/.../Canvas/...`

Use this when searching for **icons** (e.g. bleed) or TMP to mirror for custom HUD rows.
