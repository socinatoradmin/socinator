#region

using DominatorHouseCore.Interfaces.StartUp;

#endregion

namespace DominatorHouseCore.StartupActivity.Pinterest
{
    public class PinterestActivity : INetworkActivity
    {
        public BaseActivity GetActivity(string activity)
        {
            switch (activity)
            {
                case "Follow":
                case "BroadcastMessages":
                case "BoardScraper":
                case "UserScraper":
                    return new PinterestUserActivity();
                default:
                    return new PinterestPinActivity();
            }
        }
    }
}