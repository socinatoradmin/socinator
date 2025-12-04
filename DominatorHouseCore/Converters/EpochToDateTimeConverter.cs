#region

using System;
using System.Globalization;
using System.Windows.Data;
using DominatorHouseCore.Utility;

#endregion

namespace DominatorHouseCore.Converters
{
    [ValueConversion(typeof(int), typeof(DateTime))]
    public class EpochToDateTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                string interval;

                if (value.ToString().Length > 13)
                    interval = value.ToString().Substring(0, 13);
                else
                    interval = value.ToString();

                if (value.ToString().Length == 10)
                    return int.Parse(interval).EpochToDateTimeLocal();
                if (value.ToString().Length > 10)
                    return long.Parse(interval).EpochToDateTimeLocal();
                return value;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return value;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}