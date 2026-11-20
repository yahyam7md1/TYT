# Frontend — Phase 2 (Today)

**Status:** Done (mock data). Charts use LiveCharts2. No tracker / SQLite.

**Goal:** Home screen from the PRD: daily totals, top apps, distribution, empty and paused states.

---

## Already in the tree from Phase 1

- Date + previous / next (no future dates)
- Active and idle totals
- Top apps list
- Segment list
- Empty day copy
- Paused banner

## Added this phase

- NuGet: `LiveChartsCore.SkiaSharpView.WPF` (v2.2.0)
- Donut chart on Today: one slice per app (active minutes) + Idle in gray
- Chart and lists hidden when the day has no activity
- Hover tooltip uses the same duration format as the totals (`2h 15m`)

Still the same source: `IUsageQuery.GetDay` → `UsageAggregator` → list + chart. No second fake API.

---

## Files touched

- `src/Tyt.Wpf/Tyt.Wpf.csproj` — package reference
- `src/Tyt.Wpf/ViewModels/TodayViewModel.cs` — `DistributionSeries`
- `src/Tyt.Wpf/Views/TodayView.xaml` — `PieChart`

---

## Exit

- Today shows totals, top apps, and a distribution chart for the mock weekday
- Previous day shows “No activity recorded” and no chart
- Changing `MockTytState` still changes the dashboard (chart included)

---

## How the chart is wired (short)

1. `Refresh()` builds a `DaySummary` (same as the numbers).
2. `BuildDistribution` turns each app’s `TimeSpan` into a `PieSeries<double>` (value = minutes).
3. XAML: `{Binding DistributionSeries}` on `<lvc:PieChart>`.

Swap the mock later; this binding stays.

## Next

Phase 3 — Week (same segments, `GetRange`, per-day bars).
