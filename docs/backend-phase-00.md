# Backend — B0 (Windows probe)

**Status:** Done. No SQLite. Dashboard still uses mock data.

**Goal:** Prove we can ask Windows two questions: what is in front, and how long since last input.

---

## Where it went

```
src/Tyt.Infrastructure/Tracking/
  Native/User32.cs          P/Invoke (GetForegroundWindow, GetLastInputInfo)
  ForegroundAppProbe.cs     process name / exe of the focused window
  IdleProbe.cs              idle TimeSpan
  ForegroundAppSnapshot.cs  the result object

src/Tyt.Probe/              tiny console app so you can watch it
```

`Tyt.Wpf` was not changed. The UI still shows the fake Tuesday.

---

## How to run

```powershell
dotnet run --project src\Tyt.Probe
```

Then click Chrome, then VS / Cursor, then don’t touch the mouse. You should see a new “foreground →” line on each switch, and the idle seconds on the same line going up.

Ctrl+C to stop.

---

## What each file is doing

1. **User32** — C# calling Windows DLLs. Not business logic.
2. **ForegroundAppProbe** — window handle → process id → name and exe. Some processes hide the exe path; we ignore that for B0.
3. **IdleProbe** — `GetLastInputInfo` compared to `Environment.TickCount`.
4. **Tyt.Probe** — loop every 1 second and print. This is a learning tool, not the product.

## Next (not this step)

B1: turn “now VS / now idle” into open/close **segments** in memory. Still no database.
