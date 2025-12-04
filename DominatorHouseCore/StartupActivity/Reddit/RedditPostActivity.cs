#region

using System;
using System.Collections.Generic;
using System.Linq;
using DominatorHouseCore.Enums.RdQuery;
using DominatorHouseCore.Utility;

#endregion

namespace DominatorHouseCore.StartupActivity.Reddit
{
    internal class RedditPostActivity : BaseActivity
    {
        public override Type GetEnumType()
        {
            return typeof(PostQuery);
        }

        public override List<string> GetQueryType()
        {
            var listQueryType = new List<string>();
            Enum.GetValues(typeof(PostQuery)).Cast<PostQuery>().ToList().ForEach(query =>
            {
                listQueryType.Add(query.GetDescriptionAttr()?.FromResourceDictionary());
            });
            return listQueryType;
        }
    }

    internal class RedditRemoveVoteActivity : BaseActivity
    {
        public override Type GetEnumType()
        {
            return typeof(PostQuery);
        }

        public override List<string> GetQueryType()
        {
            var listQueryType = new List<string> {PostQuery.CustomUrl.GetDescriptionAttr()?.FromResourceDictionary()};
            return listQueryType;
        }
    }

    internal class RedditCommentScraperActivity : BaseActivity
    {
        public override Type GetEnumType()
        {
            return typeof(PostQuery);
        }

        public override List<string> GetQueryType()
        {
            var listQueryType = new List<string>
            {
                PostQuery.Keywords.GetDescriptionAttr()?.FromResourceDictionary(),
                PostQuery.CustomUrl.GetDescriptionAttr()?.FromResourceDictionary()
            };
            return listQueryType;
        }
    }
}