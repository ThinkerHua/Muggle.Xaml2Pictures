using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Muggle.Xaml2Pictures.ViewModels;

namespace Muggle.Xaml2Pictures.Views;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window {
    public MainWindow() {
        InitializeComponent();
    }

    private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e) {
        var listBox = (ListBox)sender;

        var selected = new List<KeyValuePairWrapper>();
        foreach (var item in listBox.SelectedItems) {
            selected.Add((KeyValuePairWrapper)item);
        }

        var vm = (MainWindowViewModel)DataContext;
        vm.SelectedDrawingGroups = selected;
    }
}