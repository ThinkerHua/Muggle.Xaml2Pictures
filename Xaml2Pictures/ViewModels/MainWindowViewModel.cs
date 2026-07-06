using System.IO;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using Muggle.Xaml2Pictures.Models;
using Muggle.Xaml2Pictures.Resources;

namespace Muggle.Xaml2Pictures.ViewModels;

public partial class MainWindowViewModel : ObservableObject {
    [ObservableProperty]
    private static double[] _dpiSet =
    [
        72, 96, 100, 150, 300, 400, 600, 1200, 2400
    ];

    [ObservableProperty]
    private static BitmapEncoderEnum[] _encoderSet =
    [
        BitmapEncoderEnum.Bmp,
        BitmapEncoderEnum.Gif,
        BitmapEncoderEnum.Jpeg,
        BitmapEncoderEnum.Png,
        BitmapEncoderEnum.Tiff,
        BitmapEncoderEnum.Wmp
    ];

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(DrawingGroups))]
    public required partial string XamlFilePath { get; set; }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ConvertDrawingGroupsToPicturesCommand))]
    public required partial string DestinationDirectory { get; set; }

    [ObservableProperty] public required partial BitmapEncoderEnum EncoderEnum { get; set; }

    [ObservableProperty] public required partial double Dpi { get; set; }
    
    [ObservableProperty] public required partial int BlandBorderWidth { get; set; }

    public IEnumerable<KeyValuePairWrapper> DrawingGroups {
        get {
            try {
                return DrawingResourceLoader.LoadAllDrawingGroups(XamlFilePath).Select(kvp => new KeyValuePairWrapper(kvp)).OrderBy(x => x.Key);
            } catch {
                return [];
            }
        }
    }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ConvertDrawingGroupsToPicturesCommand))]
    public required partial List<KeyValuePairWrapper> SelectedDrawingGroups { get; set; }

    public MainWindowViewModel()
    {
        XamlFilePath = "E:\\Documents\\Development\\Solutions\\Muggle.TsExtensions\\DC4001\\Assets\\BackgroundImages.xaml";
        DestinationDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        EncoderEnum = BitmapEncoderEnum.Bmp;
        Dpi = 96;
        BlandBorderWidth = 5;
        SelectedDrawingGroups = Enumerable.Empty<KeyValuePairWrapper>().ToList();
    }

    [RelayCommand(CanExecute = nameof(CanConvertDrawingGroupsToPicturesCommandExecute))]
    private void ConvertDrawingGroupsToPictures() {
        try {
            foreach (var drawingGroup in SelectedDrawingGroups) {
                Converter.SaveDrawingGroupWithAutoSize(
                    drawingGroup.Value,
                    Path.Combine(DestinationDirectory, $"{drawingGroup.Key}.{EncoderSuffix(EncoderEnum)}"),
                    EncoderEnum,
                    Dpi,
                    BlandBorderWidth);
            }

            MessageBox.Show(string.Format(Language.ConversionSuccessful), "", MessageBoxButton.OK, MessageBoxImage.Information);
        } catch (Exception e) {
            MessageBox.Show(string.Format(Language.ConversionFailed, e.Message), "", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private bool CanConvertDrawingGroupsToPicturesCommandExecute() {
        return SelectedDrawingGroups.Count != 0 && !string.IsNullOrEmpty(DestinationDirectory);
    }

    private static string EncoderSuffix(BitmapEncoderEnum encoder) {
        return encoder switch {
            BitmapEncoderEnum.Bmp => "bmp",
            BitmapEncoderEnum.Gif => "gif",
            BitmapEncoderEnum.Jpeg => "jpeg",
            BitmapEncoderEnum.Png => "png",
            BitmapEncoderEnum.Tiff => "tiff",
            BitmapEncoderEnum.Wmp => "wmp",
            _ => "unknown"
        };
    }

    [RelayCommand]
    private void SelectXamlFile() {
        var dialog = new OpenFileDialog {
            Title = "Select a Xaml File",
            Filter = "Xaml files (*.xaml)|*.xaml",
            FilterIndex = 0,
            DefaultExt = ".xaml",
            CheckFileExists = true,
            Multiselect = false
        };
        var result = dialog.ShowDialog();
        if (result is true) XamlFilePath = dialog.FileName;
    }

    [RelayCommand]
    private void SelectDestinationDirectory() {
        var dialog = new OpenFolderDialog() {
            Title = "Select destination directory",
            Multiselect = false
        };
        var result = dialog.ShowDialog();
        if (result is true) DestinationDirectory = dialog.FolderName;
    }
}