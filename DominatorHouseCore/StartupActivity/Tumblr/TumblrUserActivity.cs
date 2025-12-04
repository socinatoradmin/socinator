#region

using System;
using System.Collections.Generic;
using System.Linq;
using DominatorHouseCore.Enums.TumblrQuery;
using DominatorHouseCore.Utility;

#endregion

namespace DominatorHouseCore.StartupActivity.Tumblr
{
    internal class TumblrUserActivity : BaseActivity
    {
        public override Type GetEnumType()
        {
            return typeof(TumblrQuery);
        }

        public override List<string> GetQueryType()
        {
            var listQueryType = new List<string>();
            Enum.GetValues(typeof(TumblrQuery)).Cast<TumblrQuery>().ToList().ForEach(query =>
            {
                listQueryType.Add(query.GetDescriptionAttr()?.FromResourceDictionary());
            });
            return listQueryType;
        }
    }
}