# Frontend — Phase 0 and Phase 1

Status of the first frontend slice. Plan: `FRONTEND.md`. Stack: .NET 8 + WPF, mock data only.

**Status:** Phase 0 done (except close-to-tray). Phase 1 done. Extra: Today list + totals started (Phase 2 without charts).

---

## Phase 0 — Shell

**Goal:** A desktop window that feels like TYT.

**Done**

- Solution `TYT.sln` with `Tyt.Core`, `Tyt.App`, `Tyt.Infrastructure`, `Tyt.Wpf`, `Tyt.App.Tests`
- `MainWindow`: sidebar nav Today | Week | Timeline | Settings
- Right pane swaps via `CurrentPage` + DataTemplates
- Status text Tracking / Paused
- Pause / Resume button on the shell
- Week, Timeline, Settings are placeholder pages

**Not done (on purpose)**

- Close does **not** hide to tray (Phase 6). Close quits.

**Files**

- `src/Tyt.Wpf/App.xaml`
- `src/Tyt.Wpf/App.xaml.cs`
- `src/Tyt.Wpf/MainWindow.xaml`
- `src/Tyt.Wpf/MainWindow.xaml.cs`
- `src/Tyt.Wpf/ViewModels/MainViewModel.cs`
- `src/Tyt.Wpf/Views/PlaceholderView.xaml`
- `src/Tyt.Wpf/ViewModels/PlaceholderViewModel.cs`

**Exit:** Section switching works. No charts.

---

## Phase 1 — Contract + mock day

**Goal:** Frozen data shape so Today (and later Week/Timeline) all read the same segments.

**Done**

Core models:

- `AppInfo` (id, name, exe)
- `Segment` (`Kind` Active | Idle, `StartedAt`, `EndedAt` null = open)
- `TrackingSettings` (idle threshold, launch-at-startup flag)

Interfaces:

- `IUsageQuery` — `GetDay`, `GetRange`
- `ITrackingController` — pause, resume, `IsPaused`, `StateChanged`
- `ISettingsStore` — get/save settings

Mock:

- Shared `MockTytState`
- Seeded weekday: VS, Chrome, idle, Cursor, Slack, open Chrome until now
- Days with no rows return empty (previous day)
- `MockUsageQuery`, `InMemoryTrackingController`, `InMemorySettingsStore`
- Wired only in `App.xaml.cs`

Today binds to that contract:

- `TodayView` / `TodayViewModel` load `GetDay(selectedDate)`
- Raw segment list on screen (Phase 1 exit)

**Files**

- `src/Tyt.Core/Models/*`
- `src/Tyt.Core/Abstractions/IUsageQuery.cs`
- `src/Tyt.Core/Abstractions/ITrackingController.cs`
- `src/Tyt.Core/Abstractions/ISettingsStore.cs`
- `src/Tyt.Infrastructure/Mocks/MockTytState.cs`
- `src/Tyt.Infrastructure/Mocks/MockUsageQuery.cs`
- `src/Tyt.Infrastructure/Mocks/InMemoryTrackingController.cs`
- `src/Tyt.Infrastructure/Mocks/InMemorySettingsStore.cs`
- `src/Tyt.Wpf/Views/TodayView.xaml`
- `src/Tyt.Wpf/ViewModels/TodayViewModel.cs`

**Exit:** Today binds to segments.

---

## Also landed (Phase 2, incomplete)

Not required for Phase 1, already in the tree:

- Date prev/next (no future)
- Active / idle totals via `Tyt.App` `UsageAggregator`
- Top apps list
- Empty-day copy
- Paused banner on Today

**Still Phase 2:** distribution chart (e.g. LiveCharts2).

---

## Out of scope (still)

Win32 tracking, SQLite, tray, autostart registry, installer, Week/Timeline/Settings screens.

## Run

```powershell
dotnet run --project src\Tyt.Wpf
```
