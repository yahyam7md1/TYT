using System.Drawing;
using System.Windows;
using System.Windows.Forms;
using Tyt.Wpf.ViewModels;
using Application = System.Windows.Application;

namespace Tyt.Wpf.Tray;

public sealed class TrayService : IDisposable
{
    private readonly MainWindow _window;
    private readonly MainViewModel _viewModel;
    private readonly NotifyIcon _icon;
    private readonly ToolStripMenuItem _pauseItem;

    public TrayService(MainWindow window, MainViewModel viewModel)
    {
        _window = window;
        _viewModel = viewModel;

        _pauseItem = new ToolStripMenuItem();
        _pauseItem.Click += (_, _) => _viewModel.TogglePauseCommand.Execute(null);

        var menu = new ContextMenuStrip();
        menu.Items.Add("Open dashboard", null, (_, _) => ShowDashboard());
        menu.Items.Add(_pauseItem);
        menu.Items.Add(new ToolStripSeparator());
        menu.Items.Add("Quit", null, (_, _) => Quit());

        _icon = new NotifyIcon
        {
            Icon = SystemIcons.Application,
            Visible = true,
            ContextMenuStrip = menu
        };
        _icon.DoubleClick += (_, _) => ShowDashboard();
        UpdatePauseLabel();

        _viewModel.PropertyChanged += (_, args) =>
        {
            if (args.PropertyName is nameof(MainViewModel.IsPaused) or nameof(MainViewModel.StatusText))
            {
                UpdatePauseLabel();
            }
        };
    }

    public void ShowDashboard()
    {
        _window.Reveal();
    }

    public void Quit()
    {
        _window.AllowClose = true;
        _icon.Visible = false;
        Application.Current.Shutdown();
    }

    public void Dispose()
    {
        _icon.Visible = false;
        _icon.Dispose();
    }

    private void UpdatePauseLabel()
    {
        _pauseItem.Text = _viewModel.IsPaused ? "Resume tracking" : "Pause tracking";
        _icon.Text = _viewModel.IsPaused ? "TYT — Paused" : "TYT — Tracking";
    }
}
