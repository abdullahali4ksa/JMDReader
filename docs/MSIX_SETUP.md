# MSIX packaging and signing

## Current local test package
- Package identity: MDReader.MDReader
- Publisher: CN=AppPublisher
- Version: 1.0.1.0
- Architecture: x64
- Signing certificate: self-signed test certificate. Never distribute its private key.
- The public CER may accompany a test package; it does not contain the private signing key.

Build a local package using the signing certificate in the current user's certificate store:

    dotnet publish src/MDReader.App/MDReader.App.csproj -c Release -f net10.0-windows10.0.26100.0 -r win-x64 /p:GenerateAppxPackageOnBuild=true /p:AppxPackageSigningEnabled=true /p:PackageCertificateThumbprint=AF45AD5F5F15F3894DA5FAD18A44A80A8A289EC1

A self-signed package is for local testing only. Each test PC must trust the public certificate before sideloading. The provided install script asks before adding it to LocalMachine\TrustedPeople.

## Public distribution
- Microsoft Store: no developer signing certificate is required for MSIX submission; the Store signs the package after acceptance.
- Direct downloads: use a publicly trusted code-signing certificate or production signing service. Keep the private key in a secure signing environment and never commit it.
- Enterprise deployment: use an organization certificate only when its trust is deployed to all managed target devices.

Do not change manifest Publisher casually: it must match the signing identity and changing it breaks update identity.
