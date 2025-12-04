#region

using System;
using System.Collections.Generic;
using System.Linq;
using DominatorHouseCore.Enums.YdQuery;
using DominatorHouseCore.Utility;

#endregion

namespace DominatorHouseCore.StartupActivity.Youtube
{
    public class YoutubeChannelActivity : BaseActivity
    {
        public override Type GetEnumType()
        {
            return typeof(YdScraperParameters);
        }

        public override List<string> GetQueryType()
        {
            var listQueryType = new List<string>();
            Enum.GetValues(typeof(YdScraperParameters)).Cast<YdScraperParameters>().ToList().ForEach(query =>
            {
                listQueryType.Add(query.GetDescriptionAttr()?.FromResourceDictionary());
            });
            return listQueryType;
        }
    }
}