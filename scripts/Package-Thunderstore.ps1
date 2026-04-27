param(
    [string] $Configuration = "Release",
    [switch] $NoBuild
)

$ErrorActionPreference = "Stop"

$RepoRoot = Resolve-Path (Join-Path $PSScriptRoot "..")
$SolutionPath = Join-Path $RepoRoot "FairyDust.Hud.sln"
$PackageRoot = Join-Path $RepoRoot "thunderstore"
$ArtifactsRoot = Join-Path $RepoRoot "artifacts\thunderstore"
$StageRoot = Join-Path $ArtifactsRoot "stage"
$DllPath = Join-Path $RepoRoot "FairyDust.Hud\bin\$Configuration\net6.0\FairyDust.Hud.dll"
$PackagePath = Join-Path $ArtifactsRoot "FairyDust_Hud-1.0.0.zip"

if (-not $NoBuild) {
    dotnet build $SolutionPath -c $Configuration /p:SkipGameDeploy=true
}

if (-not (Test-Path -LiteralPath $DllPath)) {
    throw "Missing built DLL: $DllPath"
}

if (Test-Path -LiteralPath $StageRoot) {
    Remove-Item -LiteralPath $StageRoot -Recurse -Force
}

New-Item -ItemType Directory -Path $StageRoot | Out-Null
New-Item -ItemType Directory -Path (Join-Path $StageRoot "Mods") | Out-Null
New-Item -ItemType Directory -Path $ArtifactsRoot -Force | Out-Null

try {
    Copy-Item -LiteralPath (Join-Path $PackageRoot "manifest.json") -Destination (Join-Path $StageRoot "manifest.json") -Force
    Copy-Item -LiteralPath (Join-Path $PackageRoot "README.md") -Destination (Join-Path $StageRoot "README.md") -Force
    Copy-Item -LiteralPath (Join-Path $PackageRoot "icon.png") -Destination (Join-Path $StageRoot "icon.png") -Force
    Copy-Item -LiteralPath $DllPath -Destination (Join-Path $StageRoot "Mods\FairyDust.Hud.dll") -Force

    if (Test-Path -LiteralPath $PackagePath) {
        Remove-Item -LiteralPath $PackagePath -Force
    }

    Compress-Archive -Path (Join-Path $StageRoot "*") -DestinationPath $PackagePath -CompressionLevel Optimal
    Write-Host "Thunderstore package ready: $PackagePath"
}
finally {
    if (Test-Path -LiteralPath $StageRoot) {
        Remove-Item -LiteralPath $StageRoot -Recurse -Force
    }
}
