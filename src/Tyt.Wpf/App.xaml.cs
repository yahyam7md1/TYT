using System.Threading;
using System.Windows;
using Microsoft.Win32;
using Tyt.Infrastructure.Persistence;
using Tyt.Infrastructure.Tracking;
using Tyt.Wpf.Tray;
using Tyt.Wpf.ViewModels;

namespace Tyt.Wpf;

public partial class App : Application
{
    private Mutex? _singleInstance;
    private TrayService? _tray;
    private TytDatabase? _db;
    private TrackingController? _tracking;
    private TrackerHost? _host;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        _singleInstance = new Mutex(true, @"Local\Tyt.TrackYourTime", out var created);
        if (!created)
        {
            _singleInstance.Dispose();
            _singleInstance = null;
            MessageBox.Show("TrackYourTime is already running. Use the tray icon to open it.", "TYT");
            Shutdown();
            return;
        }

        _db = new TytDatabase(TytPaths.DatabaseFile);
        var store = new SegmentStore(_db);
        var settings = new SqliteSettingsStore(_db);
        var query = new SqliteUsageQuery(store);
        _host = new TrackerHost(store, settings, query);
        var activity = new SqliteActivityCommands(store, _host, query);
        _tracking = new TrackingController(_host, settings);
        Autostart.Apply(settings.Get().LaunchAtStartup);

        SystemEvents.PowerModeChanged += OnPowerModeChanged;
        SystemEvents.SessionEnding += OnSessionEnding;

        var main = new MainViewModel(query, _tracking, settings, activity);
        var window = new MainWindow
        {
            DataContext = main
        };
        _tray = new TrayService(window, main);
        window.Show();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        SystemEvents.PowerModeChanged -= OnPowerModeChanged;
        SystemEvents.SessionEnding -= OnSessionEnding;
        _tray?.Dispose();
        _tracking?.Dispose();
        _db?.Dispose();
        _singleInstance?.ReleaseMutex();
        _singleInstance?.Dispose();
        base.OnExit(e);
    }

    private void OnPowerModeChanged(object sender, PowerModeChangedEventArgs e)
    {
        if (e.Mode == PowerModes.Suspend)
        {
            _host?.HandleSuspend();
        }
    }

    private void OnSessionEnding(object sender, SessionEndingEventArgs e) => _host?.HandleSuspend();
}
