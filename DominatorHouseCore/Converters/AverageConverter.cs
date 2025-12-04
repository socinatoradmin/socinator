#region

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Windows.Data;

#endregion

namespace DominatorHouseCore.Converters
{
    public class AverageConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var sum = 0;
                foreach (var value in values) sum += int.Parse(value.ToString());
                return sum / values.Length;
            }
            catch
            {
                return 0;
            }
        }

        [ExcludeFromCodeCoverage]
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class NumberConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null ? (int) value : 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return 0;
        }
    }
}