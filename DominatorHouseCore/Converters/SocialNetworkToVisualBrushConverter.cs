#region

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using DominatorHouseCore.Enums;

#endregion

namespace DominatorHouseCore.Converters
{
    public class SocialNetworkToVisualBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var network = value as SocialNetworks?;
            if (network.HasValue)
                switch (network.Value)
                {
                    case SocialNetworks.Facebook:
                        return Application.Current?.FindResource("appbar_social_facebook_variant");
                    case SocialNetworks.Instagram:
                        return Application.Current?.FindResource("Instagram");
                    case SocialNetworks.Twitter:
                        return Application.Current?.FindResource("appbar_twitter_bird");
                    case SocialNetworks.Pinterest:
                        return Application.Current?.FindResource("appbar_social_pinterest");
                    case SocialNetworks.LinkedIn:
                        return Application.Current?.FindResource("appbar_social_linkedin_variant");
                    case SocialNetworks.Reddit:
                        return Application.Current?.FindResource("appbar_social_reddit");
                    case SocialNetworks.Quora:
                        return Application.Current?.FindResource("Quora");
                    //case SocialNetworks.Gplus:
                    //    return Application.Current?.FindResource("appbar_googleplus");
                    case SocialNetworks.YouTube:
                        return Application.Current?.FindResource("appbar_youtube");
                    case SocialNetworks.Tumblr:
                        return Application.Current?.FindResource("appbar_social_tumblr");
                    case SocialNetworks.Social:
                        return null;
                    case SocialNetworks.TikTok:
                        var icon = Application.Current?.FindResource("TikTok_Icon");
                        return icon;
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