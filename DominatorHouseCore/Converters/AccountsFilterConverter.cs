#region

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;

#endregion

namespace DominatorHouseCore.Converters
{
    public class AccountsFilterConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 2)
                return null;

            var collection = values[0] as IEnumerable<DominatorAccountModel>;

            var socialNetworks = values[1] as SocialNetworks?;
            var doNotSort = values.Length > 3 ? (bool) values[3] : false;
            var sortByNikeName = values.Length > 4 ? (bool) values[4] : false;
            var searchText = values.Length > 5 ? values[5].ToString() : string.Empty;

            var isReturnwithoutAssign = parameter as bool?;
            if (collection != null)
            {
                if (socialNetworks.HasValue && socialNetworks.Value != SocialNetworks.Social)
                {
                    if (isReturnwithoutAssign == true)
                        return collection.Where(a => a.AccountBaseModel.AccountNetwork == socialNetworks.Value);
                    collection = collection.Where(a => a.AccountBaseModel.AccountNetwork == socialNetworks.Value);
                }

                if (!string.IsNullOrWhiteSpace(searchText))
                    try
                    {
                        var cmbText = (int) values[6];
                        switch (cmbText)
                        {
                            case 0:
                                collection = collection.Where(x => x.UserName.IndexOf(searchText,
                                                                       StringComparison.InvariantCultureIgnoreCase) >=
                                                                   0);
                                break;
                            case 1:
                                collection = collection.Where(x =>
                                    x.AccountBaseModel.AccountGroup.Content.IndexOf(searchText,
                                        StringComparison.InvariantCultureIgnoreCase) >= 0);
                                break;
                            case 2:
                                collection = collection.Where(x => x.AccountBaseModel.AccountName.IndexOf(searchText,
                                                                       StringComparison.InvariantCultureIgnoreCase) >=
                                                                   0);
                                break;
                            case 3:
                                collection = collection.Where(x => x.AccountBaseModel.Status.GetDescriptionAttr().FromResourceDictionary().IndexOf(searchText,
                                                                       StringComparison.InvariantCultureIgnoreCase) >=
                                                                   0);
                                break;
                        }
                    }
                    catch (Exception)
                    {
                    }

                if (!doNotSort)
                {
                    if (sortByNikeName)
                        collection = collection.OrderBy(x => x.AccountBaseModel.AccountName)
                            .OrderBy(x => x.AccountBaseModel.AccountNetwork.ToString());
                    else
                        collection = collection.OrderBy(x => x.AccountBaseModel.UserName)
                            .OrderBy(x => x.AccountBaseModel.AccountNetwork.ToString());
                }
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