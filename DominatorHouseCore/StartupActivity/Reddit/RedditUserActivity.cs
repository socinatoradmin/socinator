#region

using System;
using System.Collections.Generic;
using System.Linq;
using DominatorHouseCore.Enums.RdQuery;
using DominatorHouseCore.Utility;

#endregion

namespace DominatorHouseCore.StartupActivity.Reddit
{
    internal class RedditUserActivity : BaseActivity
    {
        public override Type GetEnumType()
        {
            return typeof(UserQuery);
        }

        public override List<string> GetQueryType()
        {
            var listQueryType = new List<string>();
            Enum.GetValues(typeof(UserQuery)).Cast<UserQuery>().ToList().ForEach(query =>
            {
                listQueryType.Add(query.GetDescriptionAttr()?.FromResourceDictionary());
            });
            return listQueryType;
        }
    }
}