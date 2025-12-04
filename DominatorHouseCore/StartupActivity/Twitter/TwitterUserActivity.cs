#region

using System;
using System.Collections.Generic;
using System.Linq;
using DominatorHouseCore.Enums.TdQuery;
using DominatorHouseCore.Utility;

#endregion

namespace DominatorHouseCore.StartupActivity.Twitter
{
    internal class TwitterUserActivity : BaseActivity
    {
        public override Type GetEnumType()
        {
            return typeof(TdUserInteractionQueryEnum);
        }

        public override List<string> GetQueryType()
        {
            var listQueryType = new List<string>();
            Enum.GetValues(typeof(TdUserInteractionQueryEnum)).Cast<TdUserInteractionQueryEnum>().ToList()
                .ForEach(query => { listQueryType.Add(query.GetDescriptionAttr()?.FromResourceDictionary()); });
            return listQueryType;
        }
    }
}