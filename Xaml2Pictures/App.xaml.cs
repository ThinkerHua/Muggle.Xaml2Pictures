using System.Windows;
using Muggle.Xaml2Pictures.Views;

namespace Muggle.Xaml2Pictures;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var window = new MainWindow();
        this.MainWindow = window;
        window.Show();
    }
}