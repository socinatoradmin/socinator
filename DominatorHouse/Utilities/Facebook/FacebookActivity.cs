using DominatorHouseCore.Interfaces.StartUp;
using DominatorHouseCore.StartupActivity;

namespace DominatorHouse.Utilities.Facebook
{
    class FacebookActivity : INetworkActivity
    {
        public BaseActivity GetActivity(string activity)
        {
            switch (activity)
            {
                case "SendFriendRequest":
                    return new FacebookUserActivity();
                case "BroadcastMessages":
                case "ProfileScraper":
                    return new FacebookProfileActivity();
                case "MessageToPlaces":
                case "PlaceScraper":
                    return new FacebookPlaceActivity();
                case "GroupJoiner":
                    return new FacebookGroupJoinerActivity();
                case "GroupScraper":
                    return new FacebookGroupScraperActivity();
                case "CommentScraper":
                    return new FacebookCommentScraperActivity();
                case "LikeComment":
                case "ReplyToComment":
                    return new FacebookCommentLikerActivity();
                case "Web":
                    return new FacebookWebCommentLikerActivity();
                case "MarketPlaceScraper":
                    return new FacebookMarketPlaceActivity();
                default:
                    return new FacebookFanepageLikerActivity();
            }
        }
    }
}
