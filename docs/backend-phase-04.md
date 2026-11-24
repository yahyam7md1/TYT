# Backend — B4 (pause, settings, sleep)

**Status:** Done.

- Pause closes the current segment and skips the tracker loop. Persisted as `is_paused`.
- Idle minutes come from Settings each tick.
- Sleep / session end (`SystemEvents`) closes the open segment.
