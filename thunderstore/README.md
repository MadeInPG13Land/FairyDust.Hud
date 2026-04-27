# FAIRYDUST // DATA DECK HUD PATCH

**Module:** FAIRYDUST-HUD  
**System:** ECD-issue DataDeck (client overlay)  
**Region:** Active [Forsaken Frontiers](https://store.steampowered.com/app/2810780/Forsaken_Frontiers/) deployments - **MelonLoader** required

## Description

I've been working on the DataDeck I [found](https://thunderstore.io/c/forsaken-frontiers/p/MadeInPG13land/FairyDust_Tooltip/) on a dead ECD worker, wiring its buried operator telemetry into a field HUD that projects the wearer's vitals in front of them.

I've created a patch for immediate release to be circulated through the workforce.

The HUD adds a compact field readout for active operator conditions without opening the DataDeck.

**Details**

- **Status Board** - compact HUD readout during active deployment:
  - **HUD** - master toggle for the overlay.
  - **Stamina** - current stamina level.
  - **Bleed Out** - active bleed out timer when relevant.
  - **Infection** - active infection timer when relevant.
  - **Frostbite** - current frostbite status when relevant.
- **DataDeck Safe** - the HUD hides while the DataDeck is open.

**Notice**

This patch is not funded, authorized, or distributed by ECD or BIG CORP.
Personnel claiming otherwise are misinformed or lying.

ECD abandoned the DataDeck. We're fixing it.

## Install

**MelonLoader** - Thunderstore Mod Manager / r2modman, or manual **Mods** folder install.

## Examples

![Status HUD](https://raw.githubusercontent.com/MadeInPG13Land/FairyDust.Hud/main/thunderstore/screenshots/StatusHud.jpg)

## Config

Config is created at **`UserData/FairyDust.Hud.cfg`**.

Changes to this file apply while in game.

```text
Enabled=true
StaminaModuleEnabled=true
BleedOutModuleEnabled=true
InfectionModuleEnabled=true
FrostbiteModuleEnabled=true
```

## Changelog

### 1.0.0

- First release: **status HUD** for stamina, bleed out, infection, and frostbite.
- Added startup config card in the MelonLoader console.
- Thanks Griller for the idea to add a stamina bar to the game.

---

FAIRYDUST-HUD MK.1

UNAUTHORIZED DATADECK PATCH
