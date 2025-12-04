namespace RedditDominatorCore.RDEnums
{
    public class Enums
    {
        public enum IpLocationDetails
        {
            City = 1,
            State = 2,
            Country = 3,
            Ip = 4
        }

        public enum RdMainModule
        {
            Engage = 1,
            Voting = 2,
            GrowFollower = 3,

            UrlScraper = 4,
            PostScraper = 5,
            UserScraper = 6,

            //SubredditScraper = 7,
            GrowComment = 8,
            GrowDelete = 9,
            GrowUpvote = 10,
            GrowDownvote = 11,

            //GrowUrlScrper = 12,
            GrowRemoveVote = 13,
            GrowSubscribe = 14,
            GrowUnSubscribe = 15,
            GrowReply = 16,
            Messanger = 17,
            ChannelScraper = 18,
            CommentScraper = 19,
            AutoFeedActivity = 20
        }

        public enum WhitelistblacklistType
        {
            Group = 1,
            Private = 2
        }
    }
}