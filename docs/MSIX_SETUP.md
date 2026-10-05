# MSIX Packaging Setup (P06-T001)

Reproducible package identity and manifest configured.
Identity: MDReader (package name preserved). Version: 1.0.0.0 (reproducible, pinned).
Capabilities: `runFullTrust` only (least privilege for WinUI3 desktop).
Signing: use a development/distribution certificate outside repo; do NOT commit private keys.
Build: `dotnet publish -f net10.0-windows10.0.26100 -r win-x64 -c Release /p:GenerateAppxPackageOnBuild=true` (after configuring signing).
