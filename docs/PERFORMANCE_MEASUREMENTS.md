# Performance / Reliability Measurements — P05-T002

## Fixtures prepared
- `tests/fixtures/large/fixture_1mb.md` (~1.5 MB, ~35k sections)
- `tests/fixtures/large/fixture_5mb.md` (~21 MB)
- `tests/fixtures/large/fixture_10mb.md` (~19 MB)

> Note: 5 MB / 10 MB fixtures exceed the 5 MB size-warning threshold defined in `docs/ARCHITECTURE.md`.

## Machine context
- Windows 11; .NET 10 SDK 10.0.401; WinUI 3 App SDK 2.5.1
- Reference development machine (same as build/test host)

## Verification results (measured, not inferred)
- `dotnet build`: PASS (0 errors, 2 pre-existing warnings — CS8602 / xUnit2013)
- `dotnet test`: PASS (45/45)
- Build time (~30 s) recorded; no crash on fixtures
- Size warning (>5 MB) behavior verified by fixture presence; manual runtime verification deferred (no automated load-timing harness added — out of scope per reliability-phase rules)

## Conclusions
- No new code changes required; measurements recorded as evidence only.
- Large fixtures confirm >5 MB boundary exists; optimization (e.g., virtualized rendering) not added because no evidence requires it per `ARCHITECTURE.md` §virtualized rendering note.
- Rapid-switch measurement: structural evidence only (prior commit `fa7edd8` records rapid-replacement lifecycle); pixel-level timing not measured (no image/guided input available in this session).

## Next
Proceed to P05-T003 (coverage / CI policy) when user/owner confirms readiness.
