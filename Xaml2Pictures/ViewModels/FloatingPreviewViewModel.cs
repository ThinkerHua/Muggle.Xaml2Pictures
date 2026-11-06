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

    [ObservableProperty] private partial ViewPortWithBounds ViewPort { get; set; }

    public FloatingPreviewViewModel(string xamlFilePath, string resourceKey)
    {
        XamlFilePath = xamlFilePath;
        ResourceKey = resourceKey;
        DrawingGroup = LoadDrawingGroup();
        ViewPort = new ViewPortWithBounds(DrawingGroup.Bounds, DrawingGroup.Bounds);

        Watcher = new FileSystemWatcher(Path.GetDirectoryName(XamlFilePath)!, Path.GetFileName(XamlFilePath))
        {
            NotifyFilter = NotifyFilters.LastWrite,
            EnableRaisingEvents = true
        };
        Watcher.Changed += (_, _) =>
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                DrawingGroup = LoadDrawingGroup();
                ViewPort.Bounds = DrawingGroup.Bounds;
                OnViewPortChanged(ViewPort);
            });
        };
    }

    ~FloatingPreviewViewModel()
    {
        Watcher.Dispose();
    }

    private DrawingGroup LoadDrawingGroup()
    {
        var fallback = DrawingGroup;
        try
        {
            var originalDrawingGroup = DrawingResourceLoader
                .LoadAllDrawingGroups(XamlFilePath)
                .FirstOrDefault(kvp => kvp.Key == ResourceKey)
                .Value;
            originalDrawingGroup.Freeze();
            var drawingGroup = new DrawingGroup();
            drawingGroup.Children.Add(originalDrawingGroup);
            return drawingGroup;
        }
        catch
        {
            return fallback;
        }
    }

    partial void OnViewPortChanged(ViewPortWithBounds value)
    {
        DrawingGroup.ClipGeometry = new RectangleGeometry(ViewPort.ToRect());
        OnPropertyChanged(nameof(DrawingGroup));
    }

    public void SetRatio(double ratio)
    {
        ViewPort.Ratio = ratio;
        OnViewPortChanged(ViewPort);
    }

    public void Pan(Vector delta)
    {
        ViewPort.Pan(delta);
        OnViewPortChanged(ViewPort);
    }

    public void Scale(double scale)
    {
        ViewPort.Scale(scale);
        OnViewPortChanged(ViewPort);
    }

    public void ScaleMax()
    {
        ViewPort.ScaleMax();
        OnViewPortChanged(ViewPort);
    }

    public void ScaleAt(double scale, double percentageX, double percentageY)
    {
        ViewPort.ScaleAt(scale, ViewPort.GetPosition(percentageX, percentageY));
        OnViewPortChanged(ViewPort);
    }
}