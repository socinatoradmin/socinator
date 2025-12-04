#region

using DominatorHouseCore.Interfaces.StartUp;

#endregion

namespace DominatorHouseCore.StartupActivity.Tumblr
{
    public class TumblrActivity : INetworkActivity
    {
        public BaseActivity GetActivity(string activity)
        {
            switch (activity)
            {
                case "BroadcastMessages":
                    return new TumblrBroadcastMessageActivity();
                case "Follow":
                    return new TumblrUserActivity();
                default:
                    return new TumblrPostActivity();
            }
        }
    }
}