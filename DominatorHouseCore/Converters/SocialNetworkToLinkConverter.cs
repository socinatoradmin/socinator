#region

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Windows.Data;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;

#endregion

namespace DominatorHouseCore.Converters
{
    public class SocialNetworkToLinkConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var network = value as SocialNetworks?;
            if (network.HasValue)
                switch (network)
                {
                    case SocialNetworks.Social:
                        return ConstantVariable.SocialAccountManagerVideoLink;
                    case SocialNetworks.Facebook:
                        return ConstantVariable.FbAccountManagerVideoLink;
                    case SocialNetworks.Instagram:
                        return ConstantVariable.IgAccountManagerVideoLink;
                    case SocialNetworks.LinkedIn:
                        return ConstantVariable.LdAccountManagerVideoLink;
                    case SocialNetworks.Quora:
                        return ConstantVariable.QdAccountManagerVideoLink;
                    case SocialNetworks.Reddit:
                        return ConstantVariable.RdAccountManagerVideoLink;
                    case SocialNetworks.Tumblr:
                        return ConstantVariable.TmblrAccountManagerVideoLink;
                    case SocialNetworks.Twitter:
                        return ConstantVariable.TdAccountManagerVideoLink;
                    case SocialNetworks.Pinterest:
                        return ConstantVariable.PdAccountManagerVideoLink;
                    case SocialNetworks.YouTube:
                        return ConstantVariable.YtAccountManagerVideoLink;
                    default:
                        return ConstantVariable.SocialAccountManagerVideoLink;
                }

            return ConstantVariable.SocialAccountManagerVideoLink;
        }

        [ExcludeFromCodeCoverage]
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}