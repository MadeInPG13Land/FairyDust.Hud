# Modular structure

FairyDust.Hud uses a lightweight module/service/entity shape inspired by NestJS, adapted for a Unity MelonLoader mod. The goal is not a full dependency injection framework; the goal is boring ownership boundaries that make future HUD features easy to add without growing one giant update class.

## Top-level areas

| Folder | Owns | Should not own |
|--------|------|----------------|
| `Host/` | HUD lifecycle, canvas/dock shell, module slots, scene visibility | Feature rules, player status calculations |
| `Configuration/` | MelonLoader config, user-facing toggles, runtime paths | Layout tuning, per-frame state |
| `Game/Services/` | Safe interop with Forsaken Frontiers objects | UI rendering, feature formatting |
| `Components/` | Reusable Unity UI widgets | Game-specific status decisions |
| `Modules/` | Feature modules and their orchestration | Shared Unity plumbing |
| `Experimental/` | Quarantined code that is not part of the MVP runtime path | Normal active behavior |

## Module pattern

A module folder should contain the smallest set of pieces needed to own one feature. In this project, `Modules/Status/` is the board feature. It owns the shared status board and delegates each individual row to a submodule.

```text
Modules/
  Status/
    StatusBoardModule.cs
    Entities/
    Services/
    Stamina/
    Bleed/
    Infection/
    Frostbite/
```

`StatusBoardModule` is the only status HUD module registered with `GameplayHudHost`. It owns panel lifecycle and asks submodules for rows. It does not directly know how to calculate bleed timers, infection timers, frostbite progress, or stamina ratios.

## Status submodules

Each status row gets its own submodule folder:

```text
Modules/Status/Stamina/
  StaminaStatusModule.cs
  StaminaStatusProvider.cs
```

Use this same shape for new statuses:

```text
Modules/Status/<Name>/
  <Name>StatusModule.cs
  <Name>StatusProvider.cs
  <Name>StatusService.cs      # only if calculations grow
```

The provider decides whether the row is visible and produces a `StatusRowSnapshot`. It should not create Unity objects, touch `RectTransform`, resize slots, or call `DeckStatusPanel` directly.

## Shared status contracts

`Modules/Status/Entities/` contains the contracts between the board and status submodules:

| Type | Purpose |
|------|---------|
| `IStatusProvider` | Common provider contract for one optional row |
| `StatusProviderContext` | Player/deck context passed into providers |
| `StatusRowSnapshot` | UI-neutral row model: label, ratio, value text, warning state |
| `StatusRowKind` | Stable row identity for status-specific behavior if needed later |

These types should stay UI-neutral. If a future status needs a different visual style, prefer adding a small row-style model rather than making the deck panel check status labels.

## Services

Services are plain C# classes with explicit constructor dependencies. They should be easy to read and cheap to instantiate.

| Service | Purpose |
|---------|---------|
| `StatusBoardService` | Collects enabled provider rows in display order |
| `StatusVisibilityService` | Decides if the board should show at all |
| `StatusFormatterService` | Formats timers and value text |
| `PlayerStatusReadService` | Centralizes defensive IL2CPP/player property reads |

Keep Unity object creation out of services unless the service is explicitly a rendering service.

## Components

`Components/Deck/` is reusable UI. `DeckStatusPanel` receives generic row data and renders it. It should not know about stamina, bleed, infection, or frostbite rules.

Good component responsibilities:

- Create and destroy Unity UI objects.
- Apply Data Deck font and flavor color.
- Render labels, bars, values, background, and warning animation.
- Expose small handle methods such as `SetRow`, `ClearRows`, `SetVisible`, and `Destroy`.

Avoid component responsibilities:

- Reading `FFPlayer`.
- Checking config toggles.
- Deciding whether a status is active.
- Formatting timers.

## Adding a new status

1. Add a config toggle to `ModConfiguration`.
2. Create `Modules/Status/<Name>/`.
3. Add `<Name>StatusModule` with a `CreateProvider(...)` method.
4. Add `<Name>StatusProvider` implementing `IStatusProvider`.
5. Register the provider in `StatusBoardModule`.
6. Keep row order explicit through `SortOrder`.
7. Build with `dotnet build FairyDust.Hud.sln`.

Provider checklist:

- Return `false` when the row should be hidden.
- Return a clamped `0..1` ratio.
- Use `StatusFormatterService` for display text.
- Use `PlayerStatusReadService` for IL2CPP property reads.
- Avoid creating Unity objects or allocating avoidable per-frame helpers.

## Experimental code

`Experimental/` is for code we want to keep nearby without treating it as supported runtime architecture. Examples include the old RLPro render-texture path and deck panel variants such as pips or borders.

Experimental code should be clearly inactive by default. If an experiment becomes real again, move it out of `Experimental/`, give it lifecycle ownership, add config only if users should control it, and update these docs.
