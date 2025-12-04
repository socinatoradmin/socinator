#region

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Windows.Data;
using DominatorHouseCore.Utility;

#endregion

namespace DominatorHouseCore.Converters
{
    [ValueConversion(typeof(int), typeof(string))]
    public class IntToDateTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.ToString() == "0"
                ? "LangKeyNotUpdatedYet".FromResourceDictionary()
                : System.Convert.ToInt32(value).EpochToDateTimeLocal().ToString("dd MMM yyyy HH:mm:ss tt");
        }

        [ExcludeFromCodeCoverage]
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}