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
        using (var stream = new MemoryStream(1024))
        {
            foreach (var line in File.ReadLines(xamlFilePath))
            {
                var replaced = XNameRegex().Replace(line, string.Empty);
                var buffer = Encoding.UTF8.GetBytes(replaced);
                stream.Write(buffer, 0, buffer.Length);
            }
            
            stream.Position = 0;
            dict = (ResourceDictionary)XamlReader.Load(stream);
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