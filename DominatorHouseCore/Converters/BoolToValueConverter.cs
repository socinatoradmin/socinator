#region

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Windows.Data;

#endregion

namespace DominatorHouseCore.Converters
{
    public class BoolToValueConverter : IValueConverter
    {
        public object TrueValue { get; set; }
        public object FalseValue { get; set; }
        public object NullValue { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var boolValue = value as bool?;
            if (!boolValue.HasValue) return NullValue;

            return boolValue.Value ? TrueValue : FalseValue;
        }

        [ExcludeFromCodeCoverage]
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}