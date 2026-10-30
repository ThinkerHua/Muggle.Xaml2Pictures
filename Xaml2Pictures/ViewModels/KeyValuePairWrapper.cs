using System.Windows.Media;

namespace Muggle.Xaml2Pictures.ViewModels;

public readonly struct KeyValuePairWrapper(KeyValuePair<string, DrawingGroup> kvp)
{
    public string Key => kvp.Key;

    public DrawingGroup Value => kvp.Value;
}