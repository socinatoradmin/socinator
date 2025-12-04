#region

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using DominatorHouseCore.Enums;

#endregion

namespace DominatorHouseCore.Converters
{
    public class PinterestAccountTypeToVisibilityConverter : IValueConverter
    {
        public bool IsInverse { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (IsInverse)
                return (PinterestAccountType) value == PinterestAccountType.Inactive
                    ? Visibility.Visible
                    : Visibility.Collapsed;

            return (PinterestAccountType) value == PinterestAccountType.Active
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}