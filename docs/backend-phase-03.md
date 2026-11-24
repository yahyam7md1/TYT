# Backend — B3 (plug into the UI)

**Status:** Done.

`App.xaml.cs` now wires SQLite + `TrackerHost` instead of `MockTytState`.

Today / Week / Timeline listen to `IUsageQuery.Changed` so new segments show up without restarting.
