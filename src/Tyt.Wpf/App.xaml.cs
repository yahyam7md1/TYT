using System.Windows;
using Tyt.Infrastructure.Mocks;
using Tyt.Wpf.ViewModels;

namespace Tyt.Wpf;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var state = MockTytState.CreateSeeded();
        var main = new MainViewModel(
            new MockUsageQuery(state),
            new InMemoryTrackingController(state),
            new InMemorySettingsStore(state));

        var window = new MainWindow
        {
            DataContext = main
        };
        window.Show();
    }
}
