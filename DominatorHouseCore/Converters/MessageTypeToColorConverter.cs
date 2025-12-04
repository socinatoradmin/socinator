#region

using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;

#endregion

namespace DominatorHouseCore.Converters
{
    [ValueConversion(typeof(ChatMessageType), typeof(Brush))]
    public class MessageTypeToColorConverter : IValueConverter
    {
        public bool IsReversed { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (IsReversed)
                return (ChatMessageType) value == ChatMessageType.Media
                    ? "AccentColorBrush".FromResourceDictionary()
                    : "White";

            return (ChatMessageType) value == ChatMessageType.Media
                ? "White"
                : "AccentColorBrush".FromResourceDictionary();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}