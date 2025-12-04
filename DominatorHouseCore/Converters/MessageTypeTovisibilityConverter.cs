#region

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using DominatorHouseCore.Enums;

#endregion

namespace DominatorHouseCore.Converters
{
    [ValueConversion(typeof(ChatMessageType), typeof(Visibility))]
    public class MessageTypeTovisibilityConverter : IValueConverter
    {
        public bool IsReversed { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((ChatMessageType) value == ChatMessageType.TextAndMedia)
                return Visibility.Visible;

            if (IsReversed)
                return (ChatMessageType) value == ChatMessageType.Media ? Visibility.Visible : Visibility.Collapsed;

            return (ChatMessageType) value == ChatMessageType.Media ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}