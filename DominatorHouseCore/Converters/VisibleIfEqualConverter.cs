#region

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

#endregion

namespace DominatorHouseCore.Converters
{
    public class VisibleIfEqualConverter : IValueConverter
    {
        public object Expected { get; set; }
        public bool Inversed { get; set; }


        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var result = value?.Equals(Expected) ?? false;
            if (Inversed) return result ? Visibility.Collapsed : Visibility.Visible;

            return result ? Visibility.Visible : Visibility.Collapsed;
        }

        [ExcludeFromCodeCoverage]
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class StringEqualToVisibilityConverter : IValueConverter
    {
        public object Expected { get; set; }
        public bool Inversed { get; set; }


        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var result = value?.ToString()?.Equals(Expected) ?? false;
            if (Inversed) return result ? Visibility.Collapsed : Visibility.Visible;

            return result ? Visibility.Visible : Visibility.Collapsed;
        }

        [ExcludeFromCodeCoverage]
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}