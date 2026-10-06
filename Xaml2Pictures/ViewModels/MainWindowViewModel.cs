using System.IO;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using Muggle.Xaml2Pictures.Models;
using Muggle.Xaml2Pictures.Resources;
using Muggle.Xaml2Pictures.Views;

namespace Muggle.Xaml2Pictures.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    public static readonly double[] DpiSet =
    [
        72, 96, 100, 150, 300, 400, 600, 1200, 2400
    ];

    public static readonly BitmapEncoderEnum[] EncoderSet =
    [
        BitmapEncoderEnum.Bmp,
        BitmapEncoderEnum.Gif,
        BitmapEncoderEnum.Jpeg,
        BitmapEncoderEnum.Png,
        BitmapEncoderEnum.Tiff,
        BitmapEncoderEnum.Wmp
    ];

    public required string? XamlFilePath
    {
        get;
        set
        {
            if (!SetProperty(ref field, value)) return;

            LoadDrawingGroups();

            var directory = Path.GetDirectoryName(value) ?? string.Empty;
            var fileName = Path.GetFileName(value) ?? string.Empty;
            if (directory == string.Empty || fileName == string.Empty)
            {
                Watcher.EnableRaisingEvents = false;
            }
            else
            {
                Watcher.Path = directory;
                Watcher.Filter = fileName;
                Watcher.EnableRaisingEvents = true;
            }
        }
    }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ConvertDrawingGroupsToPicturesCommand))]
    public required partial string? DestinationDirectory { get; set; }

    [ObservableProperty] public required partial BitmapEncoderEnum EncoderEnum { get; set; }

    [ObservableProperty] public required partial double Dpi { get; set; }

    [ObservableProperty] public required partial int BlandBorderThickness { get; set; }

    [ObservableProperty] public partial KeyValuePairWrapper[] DrawingGroups { get; private set; }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ConvertDrawingGroupsToPicturesCommand))]
    public required partial KeyValuePairWrapper[] SelectedDrawingGroups { get; set; }

    private FileSystemWatcher Watcher { get; }

    [ObservableProperty] public partial bool HasExceptionWhenLoading { get; private set; }

    [ObservableProperty] public partial string ExceptionDuringLoading { get; private set; }

    public MainWindowViewModel()
    {
        DestinationDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        EncoderEnum = BitmapEncoderEnum.Bmp;
        Dpi = 96;
        BlandBorderThickness = 5;
        DrawingGroups = [];
        SelectedDrawingGroups = [];
        HasExceptionWhenLoading = false;
        ExceptionDuringLoading = string.Empty;

        Watcher = new FileSystemWatcher
        {
            NotifyFilter = NotifyFilters.LastWrite,
        };
        Watcher.Changed += (_, _) => { Application.Current.Dispatcher.Invoke(LoadDrawingGroups); };
    }

    [RelayCommand(CanExecute = nameof(CanConvertDrawingGroupsToPicturesCommandExecute))]
    private void ConvertDrawingGroupsToPictures()
    {
        try
        {
            foreach (var drawingGroup in SelectedDrawingGroups)
            {
                Converter.SaveDrawingGroupWithAutoSize(
                    drawingGroup.Value,
                    Path.Combine(DestinationDirectory!, $"{drawingGroup.Key}.{EncoderSuffix(EncoderEnum)}"),
                    EncoderEnum,
                    Dpi,
                    BlandBorderThickness);
            }

            MessageBox.Show(string.Format(Language.ConversionSuccessful), "", MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
        catch (Exception e)
        {
            MessageBox.Show(
                string.Format(Language.ConversionFailed + Environment.NewLine + Language.ErrorMessage, e.Message),
                "", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private bool CanConvertDrawingGroupsToPicturesCommandExecute()
    {
        return SelectedDrawingGroups.Length != 0 && !string.IsNullOrEmpty(DestinationDirectory);
    }

    private static string EncoderSuffix(BitmapEncoderEnum encoder)
    {
        return encoder switch
        {
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
    private void SelectXamlFile()
    {
        var dialog = new OpenFileDialog
        {
            Title = Language.SelectXamlFile,
            Filter = Language.XamlFilter,
            FilterIndex = 0,
            DefaultExt = ".xaml",
            CheckFileExists = true,
            Multiselect = false
        };
        var result = dialog.ShowDialog();
        if (result is true) XamlFilePath = dialog.FileName;
    }

    [RelayCommand]
    private void SelectDestinationDirectory()
    {
        var dialog = new OpenFolderDialog()
        {
            Title = Language.SelectDestinationDirectory,
            Multiselect = false
        };
        var result = dialog.ShowDialog();
        if (result is true) DestinationDirectory = dialog.FolderName;
    }

    private void LoadDrawingGroups()
    {
        var fallback = DrawingGroups;
        try
        {
            DrawingGroups = string.IsNullOrEmpty(XamlFilePath)
                ? []
                :
                [
                    .. DrawingResourceLoader
                        .LoadAllDrawingGroups(XamlFilePath)
                        .Select(kvp => new KeyValuePairWrapper(kvp))
                        .OrderBy(x => x.Key)
                ];

            HasExceptionWhenLoading = false;
            ExceptionDuringLoading = string.Empty;
        }
        catch (Exception e)
        {
            HasExceptionWhenLoading = true;
            ExceptionDuringLoading = e.Message;
            DrawingGroups = fallback;
        }
    }

    [RelayCommand]
    private void ShowFloatingPreview()
    {
        foreach (var drawingGroup in SelectedDrawingGroups)
        {
            var vm = new FloatingPreviewViewModel(XamlFilePath!, drawingGroup.Key);
            var view = new FloatingPreview
            {
                Title = drawingGroup.Key,
                DataContext = vm
            };
            view.Show();
        }
    }
}