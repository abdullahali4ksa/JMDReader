param(
    [string]$Subject = "CN=4B6200B3-DE4E-4F39-A6EA-B5DC049294B2",
    [string]$CertDir = "packaging/certs"
)

$ErrorActionPreference = "Stop"

if (-not (Test-Path $CertDir)) {
    New-Item -ItemType Directory -Path $CertDir -Force | Out-Null
}

$cert = Get-ChildItem -Path "Cert:\CurrentUser\My" -CodeSigningCert | Where-Object { $_.Subject -like "*$Subject*" } | Select-Object -First 1

if (-not $cert) {
    Write-Host "Creating Store Developer Certificate for $Subject..." -ForegroundColor Cyan
    $cert = New-SelfSignedCertificate -Type CodeSigningCert -Subject $Subject -CertStoreLocation "Cert:\CurrentUser\My" -NotAfter (Get-Date).AddYears(15)
    
    $pfxPassword = ConvertTo-SecureString -String "Birotech@2026!DevSecKey" -Force -AsPlainText
    Export-PfxCertificate -Cert $cert -FilePath "$CertDir/JMDReader_StoreDev.pfx" -Password $pfxPassword | Out-Null
    Export-Certificate -Cert $cert -FilePath "$CertDir/JMDReader_StoreDev.cer" | Out-Null
    Write-Host "Exported to $CertDir/JMDReader_StoreDev.pfx" -ForegroundColor Green
} else {
    Write-Host "Certificate already exists in Cert:\CurrentUser\My" -ForegroundColor Green
}

Write-Host "Thumbprint: $($cert.Thumbprint)"
Write-Host "Subject:    $($cert.Subject)"
