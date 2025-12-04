#region

using System;
using System.Collections.Generic;
using System.Linq;
using DominatorHouseCore.Enums.QdQuery;
using DominatorHouseCore.Utility;

#endregion

namespace DominatorHouseCore.StartupActivity.Quora
{
    public class QuoraFollowActivity : BaseActivity
    {
        public override Type GetEnumType()
        {
            return typeof(FollowerQuery);
        }

        public override List<string> GetQueryType()
        {
            var listQueryType = new List<string>();
            Enum.GetValues(typeof(FollowerQuery)).Cast<FollowerQuery>().ToList().ForEach(query =>
            {
                listQueryType.Add(query.GetDescriptionAttr()?.FromResourceDictionary());
            });
            return listQueryType;
        }
    }
}