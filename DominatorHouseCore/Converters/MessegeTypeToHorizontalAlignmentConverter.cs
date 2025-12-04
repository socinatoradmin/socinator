#region

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using DominatorHouseCore.Enums;

#endregion

namespace DominatorHouseCore.Converters
{
    [ValueConversion(typeof(string), typeof(HorizontalAlignment))]
    public class MessegeTypeToHorizontalAlignmentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value?.ToString() == ChatMessage.Sent.ToString()
                ? HorizontalAlignment.Right
                : HorizontalAlignment.Left;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}