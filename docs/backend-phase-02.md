# Backend — B2 (SQLite)

**Status:** Done.

File: `%LocalAppData%\TYT\tyt.db`

Tables: `apps`, `segments` (with `is_open` + checkpoint `ended_at`), `settings`.

On startup, open rows are closed at last checkpoint so a crash cannot become “until now” forever.
