#region

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;

#endregion

namespace DominatorHouseCore.Converters
{
    public sealed class ProxyManagerSourceConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 6)
                return null;
            var collection = values[0] as IEnumerable<ProxyManagerModel>;
            var showProxiesWithError = values[1] as bool?;
            var shouldUnssignedProxies = values[2] as bool?;
            var filterByGroup = values[3] as bool?;
            var group = values[4] as string;
            var filter = values[5] as string;
            var IsAllProxySelected = values[6] as bool?;

            if (collection != null)
            {
                if (showProxiesWithError ?? false)
                    collection = collection.Where(a =>
                        a.Status.IndexOf("Fail", StringComparison.InvariantCultureIgnoreCase) >= 0);

                if (shouldUnssignedProxies ?? false)
                    collection = collection.Where(a =>
                        a.AccountsAssignedto.Count == 0);

                if ((filterByGroup ?? false) && !string.IsNullOrWhiteSpace(group))
                    collection = collection.Where(a =>
                        a.AccountProxy.ProxyGroup.IndexOf(group, StringComparison.InvariantCultureIgnoreCase) >= 0);

                if (!string.IsNullOrWhiteSpace(filter))
                    collection = collection.Where(a =>
                        a.AccountProxy.ProxyIp.IndexOf(filter, StringComparison.InvariantCultureIgnoreCase) >= 0
                        || a.AccountProxy.ProxyName.IndexOf(filter, StringComparison.InvariantCultureIgnoreCase) >= 0);
            }

            if (collection != null)
            {
                collection = new ObservableCollection<ProxyManagerModel>(collection);
                if (IsAllProxySelected ?? false)
                    collection.ForEach(x => { x.IsProxySelected = true; });
            }

            return collection;
        }

        [ExcludeFromCodeCoverage]
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}