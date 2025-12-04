#region

using System;
using System.Globalization;
using System.Windows.Data;
using DominatorHouseCore.Utility;

#endregion

namespace DominatorHouseCore.Converters
{
    [ValueConversion(typeof(string), typeof(bool))]
    public class PublisherSyncStringToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null && value.ToString().Equals(ConstantVariable.NeedUpdateStatusSync);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}