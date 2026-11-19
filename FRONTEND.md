# TYT — Frontend Plan (WPF)

Companion to `PRD.mD`. Captures the agreed stack, solution layout, and **frontend-only** build phases. Tracker, SQLite, and Win32 APIs are out of scope until these phases are done.

**Status:** Draft  
**UI stack:** .NET 8 + WPF  
**Data during frontend phases:** in-memory / JSON mock behind interfaces

---

## 1. Decisions

| Topic | Choice |
|---|---|
| Platform | Windows 10 / 11 desktop |
| UI | WPF (C# + XAML), MVVM |
| Host | Single `.exe`; dashboard can close; process stays in the tray |
| Backend (later) | Same solution: Win32 foreground/idle + SQLite |
| Frontend-first | Screens bind to `IUsageQuery` / `ITrackingController` / `ISettingsStore` |
| First data | `MockUsageQuery` with a realistic day (app switches, idle gaps, open “now” segment) |

WPF was chosen so the dashboard, tray, and later tracker live in one .NET process without a browser engine (Electron) or a second UI stack (Tauri).

WinUI 3 is not the MVP UI kit (tray / window lifecycle is simpler in WPF). Electron is out (always-on RAM). A local HTTP API is out; the “backend” is C# in-process.

---

## 2. Architecture

Two jobs in one process:

1. **Dashboard (WPF)** — windows the user can close  
2. **Tracker (later)** — background loop; not built in frontend phases  

```
Views  →  ViewModels  →  interfaces (Core)  →  mock now / SQLite later
```

**Rule:** XAML and ViewModels never open SQLite or call Win32. Only `App.xaml.cs` (composition root) chooses the implementation.

Dashboard reads **segments**. Reports and charts **aggregate** segments. Do not invent a second source of truth for “hours per app.”

---

## 3. Solution structure

```
TYT.sln
├── src/
│   ├── Tyt.Core/              # models + interfaces (no WPF, no SQLite, no Win32)
│   ├── Tyt.App/               # use cases (today summary, range, pause, delete)
│   ├── Tyt.Infrastructure/    # SQLite + Windows APIs + mocks
│   └── Tyt.Wpf/               # views, viewmodels, tray
└── tests/
    └── Tyt.App.Tests/
```

### Dependencies

```
Tyt.Wpf  →  Tyt.App  →  Tyt.Core
                ↑
Tyt.Infrastructure  →  Tyt.Core
```

`Tyt.Wpf` must not use Infrastructure types except in the composition root.

### Tyt.Core

```
Tyt.Core/
├── Models/
│   ├── AppInfo.cs
│   ├── Segment.cs              # kind: Active | Idle; startedAt; endedAt (null = open)
│   └── TrackingSettings.cs
└── Abstractions/
    ├── IUsageQuery.cs          # GetDay(date), GetRange(from, to)
    ├── ITrackingController.cs  # pause, resume, is running
    └── ISettingsStore.cs
```

### Tyt.App

```
Tyt.App/
├── Dashboard/                  # today / week aggregates from segments
├── Timeline/
└── Settings/
```

### Tyt.Infrastructure

```
Tyt.Infrastructure/
├── Persistence/                # later: SQLite
├── Tracking/                   # later: foreground, idle, session writer
└── Mocks/
    └── MockUsageQuery.cs       # used for all frontend phases
```

### Tyt.Wpf

```
Tyt.Wpf/
├── App.xaml
├── App.xaml.cs                 # composition root only
├── Views/
│   ├── MainWindow.xaml
│   ├── TodayView.xaml
│   ├── WeekView.xaml
│   ├── TimelineView.xaml
│   └── SettingsView.xaml
├── ViewModels/
│   ├── MainViewModel.cs
│   ├── TodayViewModel.cs
│   ├── WeekViewModel.cs
│   ├── TimelineViewModel.cs
│   └── SettingsViewModel.cs
├── Controls/
├── Converters/
├── Tray/
│   └── TrayService.cs
└── Assets/
```

### Naming

- One view + matching viewmodel (`TodayView` / `TodayViewModel`)
- Win32 and SQL only under `Tyt.Infrastructure`
- Shared types only in `Tyt.Core` (no copy-pasted chart DTOs)
- No catch-all `Helpers.cs` / `Managers.cs`

---

## 4. Mock contract (frontend)

Use one segment list for Today, Week, and Timeline.

**Segment**

- `App` (id, display name, exe) — null/ignored when `Kind` is Idle  
- `Kind`: `Active` | `Idle`  
- `StartedAt`  
- `EndedAt`: `null` means the segment is still open (“until now”)

**Mock day should include**

- Several app switches  
- At least one idle gap (e.g. after ~90s inactivity)  
- An open last segment  
- A previous day that is empty (to test empty states)

Idle threshold, autostart flag, and paused flag live in `TrackingSettings`. During frontend phases, autostart is stored only in the mock settings object (no registry).

---

## 5. Frontend phases

Each phase is shippable UI on mock data. **Do not** add foreground hooks, SQLite, or installer work here.

### Phase 0 — Shell

**Goal:** A desktop window that feels like TYT.

- `MainWindow`, nav: Today | Week | Timeline | Settings  
- Placeholder content per section  
- Header status: Tracking vs Paused  
- Close hides the window (tray in Phase 6)

**Exit:** Section switching works. No charts required.

### Phase 1 — Contract + mock day

**Goal:** Frozen data shape.

- Core models + `IUsageQuery`, `ITrackingController`, `ISettingsStore`  
- `MockUsageQuery` with one realistic weekday + empty previous day  
- Pause/resume as in-memory flags

**Exit:** Today can bind to segments (even as a raw list).

### Phase 2 — Today

**Goal:** Home screen (PRD FR-04 / daily part of FR-05).

- Date + previous/next (no future dates)  
- Totals: active time, idle time  
- Top apps + distribution chart (one library, e.g. LiveCharts2)  
- Empty day copy  
- Paused banner

**Exit:** Changing the mock changes the dashboard. Empty previous day works.

### Phase 3 — Week

**Goal:** Weekly overview (FR-05). Same segments, different aggregation.

- Fixed week definition (choose one: Monday–Sunday **or** last 7 days)  
- Week totals; per-day active vs idle bars; top apps for the week  
- Empty week state

**Exit:** Week uses `GetRange`, not a second fake API.

### Phase 4 — Timeline

**Goal:** Chronological day (FR-06).

- Blocks for a selected day; idle visually distinct from active  
- Detail on hover/click: app, start–end, duration  
- Open segment labeled as until now  
- Empty / paused states

**Exit:** Timeline, Today totals, and top apps **match** for the same mock day.

### Phase 5 — Settings + control

**Goal:** User control (PRD), UI only.

- Idle threshold  
- Pause / resume (updates header + banners)  
- Launch at startup checkbox (persist flag only)  
- Delete all data with confirm → mock clears → empty states  
- Short privacy note: data stays on this PC

**Exit:** Pause and delete are visible across Today / Week / Timeline.

### Phase 6 — Tray and window lifecycle

**Goal:** FR-08 host behavior without a tracker.

- Tray: Open dashboard, Pause, Resume, Quit  
- Close → hide; tray Open → show  
- Optional: single instance

**Exit:** Close does not quit the process. Quit from tray does.

### Phase 7 — Polish

**Goal:** Usable MVP chrome.

- Comfortable default size (e.g. 1280×800); check 125% DPI  
- Light theme acceptable for MVP; system dark optional  
- Totals labeled (not color-only for idle vs active)

**Exit:** A new user can navigate the app on fake data without getting lost.

---

## 6. Phase order

```
0 Shell → 1 Mock contract → 2 Today → 3 Week → 4 Timeline → 5 Settings → 6 Tray → 7 Polish
```

| After | Checkpoint |
|---|---|
| Phase 2 | “Here’s how I spent Tuesday (fake).” |
| Phase 4 | Bars and timeline tell the same story. |
| Phase 6 | Behaves like a desktop app, not a browser tab. |

---

## 7. Out of scope (until frontend phases complete)

- Foreground application detection  
- Idle detection via `GetLastInputInfo`  
- SQLite persistence  
- Autostart registry / Task Scheduler  
- Sleep / shutdown session handling  
- Installer / MSIX  
- Website/tab tracking, categories, export, cloud (also out of PRD MVP)

After Phase 7, replace `MockUsageQuery` with SQLite-backed `IUsageQuery` and add `Infrastructure/Tracking` without changing View XAML.

---

**Document status:** Draft — aligned with PRD v1.0 MVP; refine if week definition or chart library is locked in.  
