#region

using System;
using System.Collections.Generic;
using System.Linq;
using DominatorHouseCore.Enums.GdQuery;
using DominatorHouseCore.Utility;

#endregion

namespace DominatorHouseCore.StartupActivity.Instagram
{
    public class InstagramUserActivity : BaseActivity
    {
        public override Type GetEnumType()
        {
            return typeof(GdUserQuery);
        }

        public override List<string> GetQueryType()
        {
            var listQueryType = new List<string>();
            Enum.GetValues(typeof(GdUserQuery)).Cast<GdUserQuery>().ToList().ForEach(query =>
            {
                listQueryType.Add(query.GetDescriptionAttr()?.FromResourceDictionary());
            });
            return listQueryType;
        }
    }

    public class InstagramUserScraperActivity : BaseActivity
    {
        public override Type GetEnumType()
        {
            return typeof(GdUserQuery);
        }

        public override List<string> GetQueryType()
        {
            var listQueryType = new List<string>();
            Enum.GetValues(typeof(GdUserQuery)).Cast<GdUserQuery>().ToList().ForEach(query =>
            {
                listQueryType.Add(query.GetDescriptionAttr()?.FromResourceDictionary());
            });
            listQueryType.Remove("Own Followers");
            listQueryType.Remove("Own Followings");
            return listQueryType;
        }
    }

    public class InstagramFollowActivity : BaseActivity
    {
        public override Type GetEnumType()
        {
            return typeof(GdUserQuery);
        }

        public override List<string> GetQueryType()
        {
            var listQueryType = new List<string>();
            Enum.GetValues(typeof(GdUserQuery)).Cast<GdUserQuery>().ToList().ForEach(query =>
            {
                listQueryType.Add(query.GetDescriptionAttr()?.FromResourceDictionary());
            });
            listQueryType.Remove("Scrap The User Who Messaged Us");
            listQueryType.Remove("Own Followers");
            listQueryType.Remove("Own Followings");
            return listQueryType;
        }
    }

    public class InstagramBroadCastActivity : BaseActivity
    {
        public override Type GetEnumType()
        {
            return typeof(GdUserQuery);
        }

        public override List<string> GetQueryType()
        {
            var listQueryType = new List<string>();
            Enum.GetValues(typeof(GdUserQuery)).Cast<GdUserQuery>().ToList().ForEach(query =>
            {
                listQueryType.Add(query.GetDescriptionAttr()?.FromResourceDictionary());
            });
            listQueryType.Remove("Scrap The User Who Messaged Us");
            return listQueryType;
        }
    }
}