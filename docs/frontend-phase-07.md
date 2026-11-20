# Frontend — Phase 7 (Polish)

**Status:** Done for MVP chrome (still mock data, light theme).

**Goal:** A new user can move around the app without getting lost.

---

## Done

- Default size 1280×800, min 960×600 (already from Phase 0; kept)
- `SnapsToDevicePixels` / `UseLayoutRounding` on the main window (125% DPI)
- Sidebar note: “Data stays on this PC”
- Default button padding
- Timeline kinds labeled **Active** / **Idle**, not color alone
- Totals still have text labels (Active / Idle) on Today and Week
- Chart legends already name series
- Empty and paused copy on Today, Week, Timeline
- Settings privacy + delete explained in plain language

**Not done (out of frontend MVP):** system dark mode, custom tray .ico, installer, real autostart.

---

## Frontend MVP complete

Phases 0–7 are implemented against mock data. Next product work is the tracker + SQLite behind the same interfaces (`IUsageQuery`, `ITrackingController`, `ISettingsStore`, `IActivityCommands`).
