#region

using DominatorHouseCore.Interfaces.StartUp;

#endregion

namespace DominatorHouseCore.StartupActivity.Twitter
{
    public class TwitterActivity : INetworkActivity
    {
        public BaseActivity GetActivity(string activity)
        {
            switch (activity)
            {
                case "Follow":
                case "Mute":
                case "UserScraper":
                case "TweetTo":
                    return new TwitterUserActivity();
                default:
                    return new TwitterTweetActivity();
            }
        }
    }
}