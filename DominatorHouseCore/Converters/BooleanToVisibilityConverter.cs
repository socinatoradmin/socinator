#region

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

#endregion

namespace DominatorHouseCore.Converters
{
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public bool IsInversed { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (IsInversed)
                return value != null && bool.Parse(value.ToString()) ? Visibility.Collapsed : Visibility.Visible;

            return value != null && bool.Parse(value.ToString()) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class VisibilityToVisibilityConverter : IValueConverter
    {
        public bool IsInversed { get; set; }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = (Visibility)value;
            return IsInversed ?
                val == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible :
                val;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}