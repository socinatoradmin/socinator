#region

using DominatorHouseCore.Interfaces.StartUp;

#endregion

namespace DominatorHouseCore.StartupActivity.Reddit
{
    public class RedditActivity : INetworkActivity
    {
        public BaseActivity GetActivity(string activity)
        {
            switch (activity)
            {
                case "Follow":
                case "BroadcastMessages":
                case "UnSubscribe":
                case "UserScraper":
                    return new RedditUserActivity();
                case "ChannelScraper":
                case "Subscribe":
                    return new RedditCommunityActivity();
                case "RemoveVote":
                case "RemoveVoteComment":
                case "Reply":
                case "UpvoteComment":
                case "DownvoteComment":
                    return new RedditRemoveVoteActivity();
                case "CommentScraper":
                    return new RedditCommentScraperActivity();
                default:
                    return new RedditPostActivity();
            }
        }
    }
}