#region

using System;
using System.Collections.Generic;
using System.Linq;
using DominatorHouseCore.Enums.TdQuery;
using DominatorHouseCore.Utility;

#endregion

namespace DominatorHouseCore.StartupActivity.Twitter
{
    internal class TwitterTweetActivity : BaseActivity
    {
        public override Type GetEnumType()
        {
            return typeof(TdTweetInteractionQueryEnum);
        }

        public override List<string> GetQueryType()
        {
            var listQueryType = new List<string>();
            Enum.GetValues(typeof(TdTweetInteractionQueryEnum)).Cast<TdTweetInteractionQueryEnum>().ToList()
                .ForEach(query => { listQueryType.Add(query.GetDescriptionAttr()?.FromResourceDictionary()); });
            return listQueryType;
        }
    }
}