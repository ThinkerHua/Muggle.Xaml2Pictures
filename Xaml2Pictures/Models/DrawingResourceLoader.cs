using System.IO;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;

namespace Muggle.Xaml2Pictures.Models;

public static class DrawingResourceLoader
{
    private static DrawingGroup? ConvertToDrawingGroup(object? resource)
    {
        switch (resource)
        {
            case DrawingGroup drawingGroup:
                return drawingGroup;
            // 处理 DrawingBrush
            case DrawingBrush drawingBrush:
                return drawingBrush.Drawing as DrawingGroup;
            // 处理 GeometryDrawing
            case GeometryDrawing geometryDrawing:
            {
                var group = new DrawingGroup();
                group.Children.Add(geometryDrawing);
                return group;
            }
            default:
                return null;
        }

    }

    public static Dictionary<string, DrawingGroup> LoadAllDrawingGroups(string xamlFilePath)
    {
        ArgumentException.ThrowIfNullOrEmpty(xamlFilePath);

        var result = new Dictionary<string, DrawingGroup>();

        ResourceDictionary dict;
        using (var fs = new FileStream(xamlFilePath, FileMode.Open, FileAccess.Read))
        {
            dict = (ResourceDictionary)XamlReader.Load(fs);
        }

        foreach (var key in dict.Keys)
        {
            if (key.ToString() is null) continue;

            var drawingGroup = ConvertToDrawingGroup(dict[key]);
            if (drawingGroup is null) continue;

            result.Add(key.ToString()!, drawingGroup);
        }

        return result;
    }
}