#region

using System;
using System.Globalization;
using System.Windows.Data;

#endregion

namespace DominatorHouseCore.Converters
{
    [ValueConversion(typeof(int), typeof(int))]
    public class ImageCountToRowHeightConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null && (int) value == 0 ? 60 : 180;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}