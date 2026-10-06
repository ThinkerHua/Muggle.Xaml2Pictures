using System.IO;
using System.Windows;
using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using Muggle.Xaml2Pictures.Models;

namespace Muggle.Xaml2Pictures.ViewModels;

public partial class FloatingPreviewViewModel : ObservableObject
{
    private string XamlFilePath { get; }

    private string ResourceKey { get; }

    [ObservableProperty] public partial DrawingGroup DrawingGroup { get; private set; }

    private FileSystemWatcher Watcher { get; }

    public FloatingPreviewViewModel(string xamlFilePath, string resourceKey)
    {
        XamlFilePath = xamlFilePath;
        ResourceKey = resourceKey;
        DrawingGroup = new DrawingGroup();

        LoadDrawingGroup();

        Watcher = new FileSystemWatcher(Path.GetDirectoryName(XamlFilePath)!, Path.GetFileName(XamlFilePath))
        {
            NotifyFilter = NotifyFilters.LastWrite,
            EnableRaisingEvents = true
        };
        Watcher.Changed += (_, _) => { Application.Current.Dispatcher.Invoke(LoadDrawingGroup); };
    }

    private void LoadDrawingGroup()
    {
        var fallback = DrawingGroup;
        try
        {
            DrawingGroup = DrawingResourceLoader
                .LoadAllDrawingGroups(XamlFilePath)
                .FirstOrDefault(kvp => kvp.Key == ResourceKey)
                .Value;
        }
        catch
        {
            DrawingGroup = fallback;
        }
    }
}