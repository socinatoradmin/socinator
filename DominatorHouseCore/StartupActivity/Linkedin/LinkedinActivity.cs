#region

using DominatorHouseCore.Interfaces.StartUp;

#endregion

namespace DominatorHouseCore.StartupActivity.Linkedin
{
    public class LinkedinActivity : INetworkActivity
    {
        public BaseActivity GetActivity(string activity)
        {
            switch (activity)
            {
                case "GroupInviter":
                case "GroupJoiner":
                    return new LinkedinGroupActivity();
                case "ConnectionRequest":
                    return new LinkedinConnectionActivity();
                case "BroadcastMessages":
                    return new LinkedinMessageActivity();
                case "UserScraper":
                    return new LinkedinScraperActivity();
                case "SalesNavigatorUserScraper":
                case "JobScraper":
                case "CompanyScraper":
                case "SalesNavigatorCompanyScraper":
                    return new LinkedinUserScraperActivity();
                default:
                    return new LinkedinEngageActivity();
            }
        }
    }
}