# TrackYourTime (TYT)

A **Windows 10/11** desktop app that shows how you spend time on your PC: which apps you used, for how long, and how much of that was idle.

Tracking runs in the background (system tray). The dashboard is optional. **All activity stays on this computer** — no account, no cloud, no screenshots, no keylogging.

---

## What it does

- Detects the **foreground app** and records how long it stays focused
- Treats **idle time** separately (no mouse/keyboard past a threshold you set)
- **Today** and **Week** summaries with charts
- **Timeline** of a day (active vs idle)
- Pause / resume, launch at Windows startup, delete local data
- Close the window to keep tracking in the tray; quit from the tray to exit

It does **not** track websites or browser tabs, take screenshots, log keys, or sync across devices.

---

## Requirements

- 64-bit **Windows 10 or 11**
- No admin rights required for normal use

---

## Download (Windows)

1. Open the repo **[Releases](https://github.com/yahyam7md1/TYT/releases)** page.
2. Download the latest `TrackYourTime-windows-x64.zip`.
3. Unzip and run **`TrackYourTime.exe`**.

Windows SmartScreen may warn that the app is unsigned. **More info → Run anyway** if you trust the build.

If there is no Release yet, build from source (below) or wait for a zip to be published.

Closing the window does **not** quit. Use the tray icon → **Quit**.

---

## Privacy and data

- Database: `%LocalAppData%\TYT\tyt.db`
- Nothing is sent to a server
- **Settings → Delete all activity data** wipes local records

---

## Build from source

You need the [.NET 8 SDK](https://dotnet.microsoft.com/download) (Windows Desktop).

```powershell
git clone https://github.com/yahyam7md1/TYT.git
cd TYT
dotnet run --project src\Tyt.Wpf
```

Publish a self-contained folder you can zip and share:

```powershell
dotnet publish src\Tyt.Wpf\Tyt.Wpf.csproj -c Release -r win-x64 --self-contained true -o publish
```

Then zip the `publish` folder. Do not commit `publish/` to git.

---

## Solution layout

| Project | Role |
|---|---|
| `Tyt.Wpf` | Dashboard, tray |
| `Tyt.Core` | Models and interfaces |
| `Tyt.App` | Daily/weekly aggregation |
| `Tyt.Infrastructure` | Win32 tracking + SQLite |
| `Tyt.Probe` | Console probe (`dotnet run --project src\Tyt.Probe`) |

---

## Status

Personal MVP: local tracking and reporting work. Not a store-signed installer. Website tracking, categories, and export are out of scope for now.
