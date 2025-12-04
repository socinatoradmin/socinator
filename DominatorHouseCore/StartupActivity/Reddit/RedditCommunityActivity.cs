#region

using System;
using System.Collections.Generic;
using System.Linq;
using DominatorHouseCore.Enums.RdQuery;
using DominatorHouseCore.Utility;

#endregion

namespace DominatorHouseCore.StartupActivity.Reddit
{
    internal class RedditCommunityActivity : BaseActivity
    {
        public override Type GetEnumType()
        {
            return typeof(CommunityQuery);
        }

        public override List<string> GetQueryType()
        {
            var listQueryType = new List<string>();
            Enum.GetValues(typeof(CommunityQuery)).Cast<CommunityQuery>().ToList().ForEach(query =>
            {
                listQueryType.Add(query.GetDescriptionAttr()?.FromResourceDictionary());
            });
            return listQueryType;
        }
    }
}