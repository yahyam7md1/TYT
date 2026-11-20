# Frontend — Phase 6 (Tray)

**Status:** Done.

**Goal:** Dashboard can close; process stays alive (PRD FR-08 host behavior).

---

## Done

- Tray icon (Windows `NotifyIcon`, default application icon)
- Double-click or **Open dashboard** → show window
- **Pause tracking** / **Resume tracking** (label follows state)
- **Quit** → real exit
- Window **X** hides the window and removes it from the taskbar (does **not** quit)
- Single instance: second launch shows a message and exits

How to stop the app: tray → Quit. Closing the window is not enough.

---

## Files

- `src/Tyt.Wpf/Tray/TrayService.cs`
- `src/Tyt.Wpf/MainWindow.xaml.cs` — `AllowClose`, `Reveal`, `OnClosing`
- `src/Tyt.Wpf/App.xaml.cs` — mutex + tray
- `src/Tyt.Wpf/Tyt.Wpf.csproj` — `UseWindowsForms`

## Next in this batch

Phase 7 — Polish.
