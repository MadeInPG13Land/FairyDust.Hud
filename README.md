# FairyDust.Hud

## WORKFORCE EQUIPMENT NOTICE: RECOVERED HUD UNIT

Recovered Data Deck peripheral now cleared for limited field deployment.

This unauthorized overlay was found in an abandoned ECD maintenance cache, still warm, still blinking, still pretending someone was coming back to finish the paperwork.

FairyDust.Hud restores a compact status readout to active personnel during Surface Operations. It does not negotiate with the weather, entities, supervisors, debt terminals, or whatever is breathing behind the Supply Officer's mask. It only tells you what your body is already trying to say.

NOTICE: This package is not affiliated with made in fairyland or the official Forsaken Frontiers development team. Dissemination has been performed by independent workforce elements.

---

## Authorized Readouts

- HUD: Enables or disables the recovered overlay.
- Stamina: Displays remaining physical output capacity.
- Bleed Out: Displays active blood-loss countdown when applicable.
- Infection: Displays infection takeover countdown when applicable.
- Frostbite: Displays exposure accumulation and projected danger state.

The unit automatically hides while the Data Deck is open to avoid interface contamination.

---

## Field Behavior

- Client-side MelonLoader mod for Forsaken Frontiers.
- Built for IL2CPP.
- Uses a Data Deck-style status board anchored near the lower-left HUD area.
- Reads local player state only.
- Does not alter stamina, infection, frostbite, bleeding, AI, economy, loot, networking, or entity behavior.

ADVISORY: If the display reports bad news, the display is not responsible for the bad news.

---

## Installation

### Thunderstore / Mod Manager

Install the package through your preferred Thunderstore-compatible manager.

### Manual

Place the mod DLL in:

```text
Forsaken Frontiers\Mods\FairyDust.Hud.dll
```

Remove any older `FairyDust.Stamina.dll` from the `Mods` folder. Duplicate legacy assemblies may cause MelonLoader to load obsolete equipment.

Required loader:

```text
MelonLoader 0.7.x
```

---

## Configuration

The config file is created at:

```text
Forsaken Frontiers\UserData\FairyDust.Hud.cfg
```

Available toggles:

```text
Enabled=true
StaminaModuleEnabled=true
BleedOutModuleEnabled=true
InfectionModuleEnabled=true
FrostbiteModuleEnabled=true
```

Config changes are checked while in game and applied automatically.

On startup, the unit prints a compact status card to the MelonLoader console:

```text
+----------------------------------------------------------+
|                      FairyDust.Hud                       |
+----------------------------------------------------------+
|  Version   1.0.0                                         |
|  File      UserData\FairyDust.Hud.cfg                    |
+----------------------------------------------------------+
|  Status Modules                                          |
|                                                          |
|  [ON]  HUD                                               |
|  [ON]  Stamina                                           |
|  [ON]  Bleed Out                                         |
|  [ON]  Infection                                         |
|  [ON]  Frostbite                                         |
+----------------------------------------------------------+
|  Auto-reload is active. Changes apply while in game.      |
+----------------------------------------------------------+
```

NOTICE: Boolean values must be `true` or `false`. Creative interpretations will be ignored.

---

## Known Limitations

- The HUD is intentionally minimal and does not provide inventory, objective, debt, location, or entity tracking.
- The overlay is tuned for standard gameplay visibility, not as an accessibility replacement for official UI.
- If the game changes player status field names in a future update, affected rows may stop appearing until the mod is updated.

---

## Release Notes

### v1.0.0 - Workforce Distribution

- Modular status board refactor cleared for deployment.
- Added stamina, bleed out, infection, and frostbite readouts.
- Added compact MelonLoader console configuration card.
- Reduced per-frame UI layout rebuilds and status update allocations.
- Added clean mod shutdown handling for MelonLoader reload cycles.
- Quarantined experimental display technology from runtime deployment.
- Prepared Thunderstore packaging workflow.

Filed under: recovered ECD equipment, questionable provenance, practical utility.
