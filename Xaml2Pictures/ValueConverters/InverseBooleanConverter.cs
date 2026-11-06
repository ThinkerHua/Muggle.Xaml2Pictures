using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Muggle.Xaml2Pictures.ValueConverters;

public class InverseBooleanConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is bool b ? !b : DependencyProperty.UnsetValue;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is bool b ? !b : Binding.DoNothing;
    }
}