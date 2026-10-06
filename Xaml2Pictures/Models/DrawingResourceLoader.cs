using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;

namespace Muggle.Xaml2Pictures.Models;

public static partial class DrawingResourceLoader
{
    private static DrawingGroup? ConvertToDrawingGroup(object? resource)
    {
        switch (resource)
        {
            case DrawingGroup drawingGroup:
                return drawingGroup;
            case DrawingBrush drawingBrush:
                return drawingBrush.Drawing as DrawingGroup;
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
        ResourceDictionary dict;
        using (var fs = new FileStream(xamlFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        using (var sr = new StreamReader(fs))
        using (var ms = new MemoryStream(1024))
        {
            while (sr.Peek() > 0)
            {
                var line = sr.ReadLine()!;
                var replaced = XNameRegex().Replace(line, string.Empty);
                var buffer = Encoding.UTF8.GetBytes(replaced);
                ms.Write(buffer, 0, buffer.Length);
            }

            ms.Position = 0;
            dict = (ResourceDictionary)XamlReader.Load(ms);
        }

        var result = new Dictionary<string, DrawingGroup>();
        foreach (var key in dict.Keys)
        {
            if (key.ToString() is null) continue;

            var drawingGroup = ConvertToDrawingGroup(dict[key]);
            if (drawingGroup is null) continue;

            result.Add(key.ToString()!, drawingGroup);
        }

        return result;
    }

    [GeneratedRegex(" x:Name=\"\\w+\"")]
    private static partial Regex XNameRegex();
}