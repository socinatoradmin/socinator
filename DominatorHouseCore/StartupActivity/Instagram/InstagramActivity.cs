#region

using DominatorHouseCore.Interfaces.StartUp;

#endregion

namespace DominatorHouseCore.StartupActivity.Instagram
{
    public class InstagramActivity : INetworkActivity
    {
        public BaseActivity GetActivity(string activity)
        {
            switch (activity)
            {
                case "DownloadScraper":
                    return new InstagramUserActivity();
                case "UserScraper":
                    return new InstagramUserScraperActivity();
                case "Follow":
                    return new InstagramFollowActivity();
                case "BroadcastMessages":
                    return new InstagramBroadCastActivity();
                case "Like":
                    return new InstagramLikeActivity();
                default:
                    return new InstagramPostActivity();
            }
        }
    }
}