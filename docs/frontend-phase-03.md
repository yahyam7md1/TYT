# Frontend — Phase 3 (Week)

**Status:** Done (mock data). Week = **Monday–Sunday**. Still no tracker / SQLite.

**Goal:** Weekly overview from the same segments as Today (`IUsageQuery.GetRange`).

---

## Done

- Week page replaces the Week placeholder
- Prev / next week (cannot go past the current week)
- Week totals: active, idle
- Grouped bar chart: active vs idle **per day** (Mon–Sun)
- Top apps for the whole week (sum of daily app time)
- Empty week: previous week has no mock rows → “No activity recorded for this week.”
- Paused banner (same flag as Today)
- Mock: extra days earlier in the current week (Mon, Tue, Thu) so bars aren’t only “today”

**Not a second API.** `GetRange(weekStart, weekEnd)` → `UsageAggregator.SummarizeWeek`.

---

## Files

- `src/Tyt.App/Dashboard/WeekCalendar.cs`
- `src/Tyt.App/Dashboard/WeekSummary.cs`
- `src/Tyt.App/Dashboard/UsageAggregator.cs` — `SummarizeWeek`
- `src/Tyt.Wpf/ViewModels/WeekViewModel.cs`
- `src/Tyt.Wpf/Views/WeekView.xaml`
- `src/Tyt.Infrastructure/Mocks/MockTytState.cs` — extra weekdays
- `src/Tyt.Wpf/ViewModels/MainViewModel.cs`
- `src/Tyt.Wpf/App.xaml` — DataTemplate
- `tests/Tyt.App.Tests/WeekCalendarTests.cs`

---

## How to read it

1. `WeekCalendar.StartOfWeek` — snap any date to Monday.
2. `WeekViewModel.Refresh` — `GetRange` then `SummarizeWeek`.
3. `WeekView.xaml` — `{Binding DailySeries}` on `CartesianChart`.

## Next

Phase 4 — Timeline (chronological blocks for one day).
