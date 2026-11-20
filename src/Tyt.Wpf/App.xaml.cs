using System.Threading;
using System.Windows;
using Tyt.Infrastructure.Mocks;
using Tyt.Wpf.Tray;
using Tyt.Wpf.ViewModels;

namespace Tyt.Wpf;

public partial class App : Application
{
    private Mutex? _singleInstance;
    private TrayService? _tray;

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

        var state = MockTytState.CreateSeeded();
        var tracking = new InMemoryTrackingController(state);
        var main = new MainViewModel(
            new MockUsageQuery(state),
            tracking,
            new InMemorySettingsStore(state),
            new InMemoryActivityCommands(state));

        var window = new MainWindow
        {
            DataContext = main
        };
        _tray = new TrayService(window, main);
        window.Show();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _tray?.Dispose();
        _singleInstance?.ReleaseMutex();
        _singleInstance?.Dispose();
        base.OnExit(e);
    }
}
