<#
.SYNOPSIS
    Compiles the Inno Setup wizard into dist/installer/MDReader-Setup-x64.exe and signs it.
#>

param(
    [switch]$SkipBuild = $false
)

$ErrorActionPreference = "Stop"

Write-Host "==========================================================" -ForegroundColor Cyan
Write-Host "  MDReader — Standard Windows Setup Wizard Compiler       " -ForegroundColor Cyan
Write-Host "==========================================================" -ForegroundColor Cyan

# 1. Check or build binaries
if (-not $SkipBuild -or -not (Test-Path "dist/win-x64/MDReader.App.exe")) {
    Write-Host "Triggering release publish first..." -ForegroundColor Yellow
    powershell -ExecutionPolicy Bypass -File scripts/build-release.ps1
}

# 2. Sign all binaries in dist/win-x64
Write-Host "Signing application binaries prior to packaging..." -ForegroundColor Cyan
powershell -ExecutionPolicy Bypass -File scripts/sign-binaries.ps1 -TargetDir "dist/win-x64"

# 3. Locate ISCC.exe
$isccCandidates = @(
    "$env:LOCALAPPDATA\Programs\Inno Setup 6\ISCC.exe",
    "${env:ProgramFiles(x86)}\Inno Setup 6\ISCC.exe",
    "$env:ProgramFiles\Inno Setup 6\ISCC.exe"
) | Where-Object { $_ -and (Test-Path $_) }

if (-not $isccCandidates -or @($isccCandidates).Count -eq 0) {
    Write-Error "Inno Setup compiler (ISCC.exe) not found! Please ensure Inno Setup 6 is installed."
}

$isccPath = @($isccCandidates)[0]
Write-Host "Using Inno Setup compiler: $isccPath" -ForegroundColor Green

# 4. Compile setup.iss
$issFile = "packaging/inno/setup.iss"
if (-not (Test-Path $issFile)) {
    Write-Error "Inno Setup script not found at $issFile"
}

$installerOutputDir = "dist/installer"
if (-not (Test-Path $installerOutputDir)) {
    New-Item -ItemType Directory -Path $installerOutputDir -Force | Out-Null
}

Write-Host "Compiling installer wizard from $issFile..." -ForegroundColor Cyan
& $isccPath $issFile

if ($LASTEXITCODE -ne 0) {
    Write-Error "ISCC compiler failed with exit code $LASTEXITCODE"
}

# 5. Sign the generated Setup installer
$installerFile = Join-Path $installerOutputDir "MDReader-Setup-x64.exe"
if (Test-Path $installerFile) {
    Write-Host "Signing final setup wizard: $installerFile..." -ForegroundColor Cyan
    powershell -ExecutionPolicy Bypass -File scripts/sign-binaries.ps1 -TargetDir $installerOutputDir
    
    $fileInfo = Get-Item $installerFile
    $hash = (Get-FileHash -Path $installerFile -Algorithm SHA256).Hash
    Write-Host ""
    Write-Host "==========================================================" -ForegroundColor Green
    Write-Host "  SETUP WIZARD COMPILED AND SIGNED SUCCESSFULLY!         " -ForegroundColor Green
    Write-Host "==========================================================" -ForegroundColor Green
    Write-Host "File:   $($fileInfo.FullName)" -ForegroundColor White
    Write-Host "Size:   $([Math]::Round($fileInfo.Length / 1MB, 2)) MB" -ForegroundColor White
    Write-Host "SHA256: $hash" -ForegroundColor White
} else {
    Write-Error "Expected installer file $installerFile was not produced."
}
