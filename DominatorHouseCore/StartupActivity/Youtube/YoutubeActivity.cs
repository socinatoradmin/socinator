#region

using DominatorHouseCore.Interfaces.StartUp;

#endregion

namespace DominatorHouseCore.StartupActivity.Youtube
{
    public class YoutubeActivity : INetworkActivity
    {
        public BaseActivity GetActivity(string activity)
        {
            return new YoutubeChannelActivity();
        }
    }
}