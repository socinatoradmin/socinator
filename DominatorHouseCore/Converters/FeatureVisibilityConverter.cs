#region

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;

#endregion

namespace DominatorHouseCore.Converters
{
    [ValueConversion(typeof(SocialNetworks), typeof(Visibility))]
    public class FeatureVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return Visibility.Collapsed;

            var network = value.ToString();
            if (string.IsNullOrEmpty(network))
                return Visibility.Collapsed;

            var visibilityStatus = FeatureFlags.Check(network) ? Visibility.Visible : Visibility.Collapsed;

            return visibilityStatus;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Visibility.Collapsed;
        }
    }
}