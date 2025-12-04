using System.Collections.Generic;

namespace GramDominatorCore.GDModel
{
    public class UserInfoWithFeed
    {
        public UserInfoWithFeed(InstagramUser user, List<InstagramPost> feed)
        {
            User = user;
            Feed = feed;
        }

        public List<InstagramPost> Feed { get; }

        public InstagramUser User { get; }
    }
}
