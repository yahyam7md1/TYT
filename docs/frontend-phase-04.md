# Frontend — Phase 4 (Timeline)

**Status:** Done (mock data).

**Goal:** Chronological day view (PRD FR-06) from the same `GetDay` segments as Today.

---

## Done

- Timeline page (replaces placeholder)
- Date prev/next (same rules as Today)
- One block per segment, in time order
- **Active** = blue bar + “Active” tag; **Idle** = gray bar + “Idle” tag (not color-only)
- Open segment labeled **Until now**
- Hover tooltip: name, kind, range, duration
- Click a block → detail panel at the bottom
- Empty day and paused banner
- Active total for the day (should match Today’s active total for that date)

---

## Files

- `src/Tyt.Wpf/ViewModels/TimelineViewModel.cs`
- `src/Tyt.Wpf/ViewModels/TimelineBlock.cs`
- `src/Tyt.Wpf/Views/TimelineView.xaml`
- `src/Tyt.Wpf/Converters/NullToVisibilityConverter.cs`

## Next in this batch

Phase 5 — Settings.
