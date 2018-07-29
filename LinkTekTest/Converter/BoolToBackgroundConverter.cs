using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace LinkTekTest.Converter
{
    /// <summary>
    /// Intended for indicating a changed (and unsaved) field in the UI.
    /// </summary>
    [ValueConversion(typeof(bool), typeof(SolidColorBrush))]
    internal class BoolToBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || !(bool)value)
                return new SolidColorBrush(Colors.White);

            return new SolidColorBrush(Color.FromRgb(218, 230, 240));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
