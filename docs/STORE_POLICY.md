# Store Policy, Privacy, and Capability Review (P07-T001)

## Manifest capabilities review
- `runFullTrust`: justified (WinUI 3 native desktop); no broader capabilities requested.
- No network, file-system beyond app container, or device access declared beyond what WinUI needs.

## Privacy / EULA disclosure
- Offline reader only; no telemetry; no account; no cloud sync; no ads; no data collection.
- Privacy statement: no user data leaves the device except via explicit OS file picker/open.
- No registration, no license server, no analytics endpoint.

## Certification checklist (prepared, not submitted)
- Manifest valid; least-privilege capabilities; identity pinned.
- Description and display name consistent.
- No remote image/network fetch; HTML inert.
- Signing uses out-of-repo dev cert (documented in MSIX_SETUP.md).
- Actual Store submission remains a separate owner authorization.
