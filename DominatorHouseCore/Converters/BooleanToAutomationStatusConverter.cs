#region

using System;
using System.Globalization;
using System.Windows.Data;
using DominatorHouseCore.Utility;

#endregion

namespace DominatorHouseCore.Converters
{
    public class BooleanToAutomationStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool) value ? "LangKeyActive".FromResourceDictionary() : "LangKeyInActive".FromResourceDictionary();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return "LangKeyInActive".FromResourceDictionary();
        }
    }
}