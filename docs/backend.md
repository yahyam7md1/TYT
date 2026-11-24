# TYT backend

Runs **in the same exe** as the dashboard (`Tyt.Infrastructure`). Not a web server.

| Phase | Doc | What |
|---|---|---|
| B0 | [backend-phase-00.md](backend-phase-00.md) | Foreground + idle probes |
| B1 | [backend-phase-01.md](backend-phase-01.md) | `SessionEngine` open/close rules |
| B2 | [backend-phase-02.md](backend-phase-02.md) | SQLite at `%LocalAppData%\TYT\tyt.db` |
| B3 | [backend-phase-03.md](backend-phase-03.md) | UI uses real query + tracker |
| B4 | [backend-phase-04.md](backend-phase-04.md) | Pause, idle setting, sleep |
| B5 | [backend-phase-05.md](backend-phase-05.md) | HKCU autostart |

Loop (1s): idle? → `SessionEngine` → SQLite on switch / every 30s checkpoint.

Learning probe still works: `dotnet run --project src\Tyt.Probe`
