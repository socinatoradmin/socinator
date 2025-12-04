#region

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using DominatorHouseCore.Enums;

#endregion

namespace DominatorHouseCore.Converters
{
    public class ActivityTypesSourceConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var collection = values[0] as IEnumerable<ActivityType?>;
            var network = values[1] as SocialNetworks?;

            if (network == null || network == SocialNetworks.Social) return collection;

            return collection.Where(a => a?.IsSupportedByNetwork(network.Value) ?? false).ToList();
        }

        [ExcludeFromCodeCoverage]
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}