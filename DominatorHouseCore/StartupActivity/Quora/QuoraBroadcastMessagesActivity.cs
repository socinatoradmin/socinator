#region

using System;
using System.Collections.Generic;
using System.Linq;
using DominatorHouseCore.Enums.QdQuery;
using DominatorHouseCore.Utility;

#endregion

namespace DominatorHouseCore.StartupActivity.Quora
{
    public class QuoraBroadcastMessagesActivity : BaseActivity
    {
        public override Type GetEnumType()
        {
            return typeof(BroadcastMessageQuery);
        }

        public override List<string> GetQueryType()
        {
            var listQueryType = new List<string>();
            Enum.GetValues(typeof(BroadcastMessageQuery)).Cast<BroadcastMessageQuery>().ToList().ForEach(query =>
            {
                listQueryType.Add(query.GetDescriptionAttr()?.FromResourceDictionary());
            });
            return listQueryType;
        }
    }
}