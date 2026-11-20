# Frontend — Phase 5 (Settings)

**Status:** Done (UI only; no registry, no SQLite).

**Goal:** User-visible control from the PRD.

---

## Done

- Idle threshold slider (1–60 minutes), saved on `ISettingsStore` (in-memory)
- Pause / resume on the Settings page (same `ITrackingController` as the sidebar)
- Launch at Windows startup **checkbox** — flag only, no registry
- Delete all activity data with confirm dialog → mock segment list cleared → Today / Week / Timeline empty
- Privacy copy: data stays on this PC; no screenshots, keystrokes, or cloud

New contract: `IActivityCommands.DeleteAll()` implemented by `InMemoryActivityCommands`.

Pause and delete notify `MockTytState.Changed`, which is the same event the dashboards already listen to.

---

## Files

- `src/Tyt.Core/Abstractions/IActivityCommands.cs`
- `src/Tyt.Infrastructure/Mocks/InMemoryActivityCommands.cs`
- `src/Tyt.Wpf/ViewModels/SettingsViewModel.cs`
- `src/Tyt.Wpf/Views/SettingsView.xaml`

## Next in this batch

Phase 6 — Tray.
