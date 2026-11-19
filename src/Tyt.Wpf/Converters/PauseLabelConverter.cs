using System.Globalization;
using System.Windows.Data;

namespace Tyt.Wpf.Converters;

public sealed class PauseLabelConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
        value is true ? "Resume tracking" : "Pause tracking";

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}
