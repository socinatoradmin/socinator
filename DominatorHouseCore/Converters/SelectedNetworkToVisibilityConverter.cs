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
    public class SelectedNetworkToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (SocialNetworks) value == SocialNetworks.Pinterest ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return "LangKeyInActive".FromResourceDictionary();
        }
    }
}