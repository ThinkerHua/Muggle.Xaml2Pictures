using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Muggle.Xaml2Pictures.Resources;

namespace Muggle.Xaml2Pictures.ValueConverters;

public class BoundsToStringValueConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is Rect rect 
            ? string.Format(Language.BoundsFormat, rect.X, rect.Y, rect.Width, rect.Height) 
            : DependencyProperty.UnsetValue;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return Binding.DoNothing;
    }
}