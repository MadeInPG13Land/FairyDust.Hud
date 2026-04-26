# FairyDust.Template

Minimal Forsaken Frontiers IL2CPP MelonLoader template.

## Layout

- `FairyDust.Template/` contains the mod project.
- `FairyDust.Template/Configuration/` contains the MelonLoader-backed config and environment bootstrap.

## Startup Shape

- `Main.cs` is the MelonLoader entrypoint and startup bootstrap.
- `Config.cs` holds the typed MelonLoader-backed config object and save flow.
- `FairyDustEnvironment.cs` centralizes runtime paths.

## Configuration

This template uses MelonLoader's built-in preferences system through a typed config wrapper.
`Config.Initialize()` loads `Config.Values` from `UserData/FairyDust.Template.cfg`, and `Config.Save()` persists changes back to that file.

## Environment

`FairyDustEnvironment` centralizes the template's common runtime paths in the same spirit as MelonLoader's `MelonEnvironment`, so code can read stable properties instead of rebuilding paths ad hoc.

## Local Setup

Default game path:

`C:\Program Files (x86)\Steam\steamapps\common\Forsaken Frontiers`

Override local paths with `Local.Build.props` if needed.

## MelonLoader Attributes

The template currently includes:

- `MelonInfo`
- `MelonColor(255, 244, 155, 171)`
- `MelonAuthorColor(255, 155, 126, 189)`
- `MelonGame("made in fairyland", "Forsaken Frontiers")`
- `MelonProcess("Forsaken Frontiers.exe")`
- `MelonPlatformDomain(MelonPlatformDomainAttribute.CompatibleDomains.IL2CPP)`

The IL2CPP domain attribute makes the template's intended runtime target explicit alongside normal game and process matching.

## Commands

```powershell
dotnet build
dotnet format
```
