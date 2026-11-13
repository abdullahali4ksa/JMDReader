<#
.SYNOPSIS
    Builds a standalone self-contained release of MDReader for win-x64.
.DESCRIPTION
    Publishes MDReader.App as a self-contained win-x64 application with all assets,
    offline renderer files, and runtime dependencies placed into dist/win-x64.
#>

param(
    [string]$Configuration = "Release",
    [string]$OutputDir = "dist/win-x64"
)

$ErrorActionPreference = "Stop"

Write-Host "==========================================================" -ForegroundColor Cyan
Write-Host "  MDReader — Standalone Release Build (win-x64)           " -ForegroundColor Cyan
Write-Host "==========================================================" -ForegroundColor Cyan

# 1. Clean output folder
if (Test-Path $OutputDir) {
    Write-Host "Cleaning existing output at $OutputDir..." -ForegroundColor Yellow
    Remove-Item -Path $OutputDir -Recurse -Force
}

New-Item -ItemType Directory -Path $OutputDir -Force | Out-Null

# 2. Publish self-contained executable
Write-Host "Publishing MDReader.App ($Configuration, win-x64, self-contained)..." -ForegroundColor Cyan
dotnet publish src/MDReader.App/MDReader.App.csproj `
    -c $Configuration `
    -r win-x64 `
    --self-contained true `
    /p:PublishSingleFile=false `
    /p:PublishTrimmed=false `
    -o $OutputDir

if ($LASTEXITCODE -ne 0) {
    Write-Error "dotnet publish failed with exit code $LASTEXITCODE"
}

# 3. Ensure Assets and Renderer are present in the output
$assetsSrc = "src/MDReader.App/Assets"
$assetsDest = Join-Path $OutputDir "Assets"

if (-not (Test-Path (Join-Path $assetsDest "AppIcon.ico"))) {
    Write-Host "Copying Assets folder to $assetsDest..." -ForegroundColor Yellow
    Copy-Item -Path $assetsSrc -Destination $OutputDir -Recurse -Force
}

# 4. Verify primary executable exists
$exePath = Join-Path $OutputDir "MDReader.App.exe"
if (Test-Path $exePath) {
    $exeSize = (Get-Item $exePath).Length / 1KB
    Write-Host "SUCCESS: $exePath generated ($([Math]::Round($exeSize, 2)) KB)" -ForegroundColor Green
} else {
    Write-Error "Primary executable $exePath was not found after publish!"
}
