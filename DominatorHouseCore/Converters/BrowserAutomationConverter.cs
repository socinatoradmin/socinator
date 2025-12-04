#region

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

#endregion

namespace DominatorHouseCore.Converters
{
    public class BrowserAutomationConverter : IValueConverter
    {
        public bool IsInverse { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var isTrue = (bool) value;

            if (isTrue && parameter.Equals("Browser"))
                return Visibility.Collapsed;
            if (!isTrue && parameter.Equals("http"))
                return Visibility.Collapsed;

            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}