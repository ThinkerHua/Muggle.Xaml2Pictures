using System.Windows;
using System.Windows.Controls;
using Muggle.Xaml2Pictures.ViewModels;

namespace Muggle.Xaml2Pictures.Views;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var listBox = (ListBox)sender;

        var selected = new KeyValuePairWrapper[listBox.SelectedItems.Count];
        for (var i = 0; i < listBox.SelectedItems.Count; i++)
        {
            var item = listBox.SelectedItems[i];
            selected[i] = (KeyValuePairWrapper)item!;
        }

        var vm = (MainWindowViewModel)DataContext;
        vm.SelectedDrawingGroups = selected;
    }
}