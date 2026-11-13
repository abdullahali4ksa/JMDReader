param(
    [string]$TargetDir = "dist/win-x64",
    [string]$CertSubject = "CN=birotech, O=birotech, C=SA"
)

# 1. Find or create master developer code signing certificate for birotech
$existingCert = Get-ChildItem -Path "Cert:\CurrentUser\My" -CodeSigningCert | Where-Object { $_.Subject -like "*CN=birotech*" } | Select-Object -First 1

if (-not $existingCert) {
    Write-Host "Creating new Master Developer Certificate for birotech..." -ForegroundColor Cyan
    $existingCert = New-SelfSignedCertificate -Type CodeSigningCert -Subject $CertSubject -CertStoreLocation "Cert:\CurrentUser\My" -NotAfter (Get-Date).AddYears(15)
    
    # Export PFX to packaging/certs
    $certDir = "packaging/certs"
    if (-not (Test-Path $certDir)) { New-Item -ItemType Directory -Path $certDir -Force | Out-Null }
    $pfxPassword = ConvertTo-SecureString -String "Birotech@2026!DevSecKey" -Force -AsPlainText
    Export-PfxCertificate -Cert $existingCert -FilePath "$certDir/birotech_Dev.pfx" -Password $pfxPassword | Out-Null
    Write-Host "Exported Master Developer Certificate to $certDir/birotech_Dev.pfx" -ForegroundColor Green
}

Write-Host "Using certificate: $($existingCert.Thumbprint) ($($existingCert.Subject))" -ForegroundColor Green

# 2. Sign all .exe and .dll files in TargetDir
$filesToSign = Get-ChildItem -Path $TargetDir -Recurse -Include "*.exe", "*.dll"

Write-Host "Signing $($filesToSign.Count) binaries in $TargetDir..." -ForegroundColor Cyan
$successCount = 0

foreach ($file in $filesToSign) {
    try {
        $sig = Set-AuthenticodeSignature -FilePath $file.FullName -Certificate $existingCert -HashAlgorithm SHA256
        if ($sig.Status -eq "Valid" -or $sig.Status -eq "UnknownError") {
            $successCount++
        }
    }
    catch {
        Write-Warning "Failed to sign $($file.Name): $_"
    }
}

Write-Host "Successfully signed $successCount / $($filesToSign.Count) binaries!" -ForegroundColor Green
