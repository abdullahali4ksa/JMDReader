<#
.SYNOPSIS
    Builds and signs a Windows MSIX Package (MDReader.msix).
.DESCRIPTION
    Packages MDReader.App into a modern signed MSIX container using
    the developer code signing certificate.
#>

param(
    [string]$Configuration = "Release",
    [string]$OutputDir = "dist/msix"
)

$ErrorActionPreference = "Stop"

Write-Host "==========================================================" -ForegroundColor Cyan
Write-Host "  MDReader — Native Windows MSIX Package Compiler         " -ForegroundColor Cyan
Write-Host "==========================================================" -ForegroundColor Cyan

# 1. Ensure cert exists
$cert = Get-ChildItem -Path "Cert:\CurrentUser\My" -CodeSigningCert | Where-Object { $_.Subject -like "*CN=4B6200B3*" -or $_.Subject -like "*CN=birotech*" } | Select-Object -First 1

if (-not $cert) {
    Write-Host "Certificate not found, running setup-store-cert.ps1..." -ForegroundColor Yellow
    powershell -ExecutionPolicy Bypass -File scripts/setup-store-cert.ps1
    $cert = Get-ChildItem -Path "Cert:\CurrentUser\My" -CodeSigningCert | Where-Object { $_.Subject -like "*CN=4B6200B3*" } | Select-Object -First 1
}

$fullOutputDir = (New-Item -ItemType Directory -Path $OutputDir -Force).FullName

Write-Host "Packaging MSIX to $fullOutputDir with certificate thumbprint $($cert.Thumbprint)..." -ForegroundColor Cyan

# 2. Publish MSIX Package
dotnet publish src/MDReader.App/MDReader.App.csproj `
    -c $Configuration `
    -r win-x64 `
    /p:GenerateAppxPackageOnBuild=true `
    /p:AppxPackageDir="$fullOutputDir\" `
    /p:AppxBundle=Never `
    /p:UapAppxPackageBuildMode=SideloadOnly `
    /p:AppxPackageSigningEnabled=true `
    /p:PackageCertificateThumbprint="$($cert.Thumbprint)" `
    /p:PublishTrimmed=false

if ($LASTEXITCODE -ne 0) {
    Write-Error "MSIX packaging failed with exit code $LASTEXITCODE"
}

# 3. Locate generated msix
$msixFiles = Get-ChildItem -Path $fullOutputDir -Filter "*.msix" -Recurse
if ($msixFiles.Count -gt 0) {
    $msix = $msixFiles[0]
    $sizeMb = [Math]::Round($msix.Length / 1MB, 2)
    $hash = (Get-FileHash -Path $msix.FullName -Algorithm SHA256).Hash
    Write-Host ""
    Write-Host "==========================================================" -ForegroundColor Green
    Write-Host "  MSIX PACKAGE BUILT AND SIGNED SUCCESSFULLY!             " -ForegroundColor Green
    Write-Host "==========================================================" -ForegroundColor Green
    Write-Host "Package: $msix" -ForegroundColor White
    Write-Host "Size:    $sizeMb MB" -ForegroundColor White
    Write-Host "SHA256:  $hash" -ForegroundColor White
} else {
    Write-Warning "No .msix file found in $fullOutputDir. Check msix subdirectory output."
}
