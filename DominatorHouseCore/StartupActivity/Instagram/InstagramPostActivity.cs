#region

using System;
using System.Collections.Generic;
using System.Linq;
using DominatorHouseCore.Enums.GdQuery;
using DominatorHouseCore.Utility;

#endregion

namespace DominatorHouseCore.StartupActivity.Instagram
{
    public class InstagramPostActivity : BaseActivity
    {
        public override Type GetEnumType()
        {
            return typeof(GdPostQuery);
        }

        public override List<string> GetQueryType()
        {
            var listQueryType = new List<string>();
            Enum.GetValues(typeof(GdPostQuery)).Cast<GdPostQuery>().ToList().ForEach(query =>
            {
                listQueryType.Add(query.GetDescriptionAttr()?.FromResourceDictionary());
            });

            return listQueryType;
        }
    }

    public class InstagramLikeActivity : BaseActivity
    {
        public override Type GetEnumType()
        {
            return typeof(GdUserQuery);
        }

        public override List<string> GetQueryType()
        {
            var listQueryType = new List<string>();
            Enum.GetValues(typeof(GdPostQuery)).Cast<GdPostQuery>().ToList().ForEach(query =>
            {
                listQueryType.Add(query.GetDescriptionAttr()?.FromResourceDictionary());
            });
            listQueryType.Remove("Own Liked Post");
            return listQueryType;
        }
    }
}