# Store Listing Assets — P07-T002

Prepared for review; submission is a separate authorized action.

## Listing description (draft)
- Name: MDReader
- Subtitle: Native Markdown reader for Windows
- Description: A small, fast, native WinUI 3 app to open and read .md files. Offline, no account, no telemetry.
- Keywords: markdown, reader, winui, windows, native

## Assets needed (not submitted)
- Icon set: 44x44, 150x150, 71x71, 310x310 tiles
- Screenshots: 1-2 showing light/dark rendering of a .md file
- Trailer video: not required for MVP

## Submission-ready build steps (documented, not executed)
- `dotnet publish -f net10.0-windows10.0.26100 -r win-x64 -c Release /p:GenerateAppxPackageOnBuild=true`
- Use out-of-repo signing cert (see MSIX_SETUP.md)
- No private keys in repo.

## Unresolved / owner authorization
- Actual Store submission waits for separate owner authorization.
- Listing screenshots and icon assets not produced in repo.
