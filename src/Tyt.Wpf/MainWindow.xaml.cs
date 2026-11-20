using System.ComponentModel;
using System.Windows;

namespace Tyt.Wpf;

public partial class MainWindow : Window
{
    public bool AllowClose { get; set; }

    public MainWindow()
    {
        InitializeComponent();
    }

    public void Reveal()
    {
        Show();
        ShowInTaskbar = true;
        if (WindowState == WindowState.Minimized)
        {
            WindowState = WindowState.Normal;
        }

        Activate();
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        if (!AllowClose)
        {
            e.Cancel = true;
            Hide();
            ShowInTaskbar = false;
            return;
        }

        base.OnClosing(e);
    }
}
