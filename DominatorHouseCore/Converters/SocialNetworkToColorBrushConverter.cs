#region

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using DominatorHouseCore.Enums;

#endregion

namespace DominatorHouseCore.Converters
{
    public class SocialNetworkToColorBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var network = value as SocialNetworks?;
            if (network.HasValue)
                switch (network.Value)
                {
                    case SocialNetworks.Facebook:
                        return (SolidColorBrush) new BrushConverter().ConvertFrom("#3684C1");
                    case SocialNetworks.Instagram:
                        return (SolidColorBrush) new BrushConverter().ConvertFrom("#8a3ba8");
                    //return Brushes.Red;
                    case SocialNetworks.Twitter:
                        return (SolidColorBrush) new BrushConverter().ConvertFrom("#3897F0");
                    case SocialNetworks.Pinterest:
                        return (SolidColorBrush) new BrushConverter().ConvertFrom("#e60023");
                    case SocialNetworks.Quora:
                        return (SolidColorBrush) new BrushConverter().ConvertFrom("#b92b27");
                    //return Brushes.Red;
                    case SocialNetworks.LinkedIn:
                        return Brushes.DodgerBlue;
                    case SocialNetworks.Reddit:
                        return Brushes.OrangeRed;
                    //case SocialNetworks.Gplus:
                    //    return (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF0000"));
                    case SocialNetworks.YouTube:
                        return (SolidColorBrush) new BrushConverter().ConvertFrom("#fd0000");
                    case SocialNetworks.Tumblr:
                        return Brushes.DimGray;
                    case SocialNetworks.TikTok:
                        return Brushes.DarkCyan;
                    case SocialNetworks.Social:
                        return null;

                    default:
                        throw new ArgumentOutOfRangeException();
                }

            return null;
        }

        [ExcludeFromCodeCoverage]
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}