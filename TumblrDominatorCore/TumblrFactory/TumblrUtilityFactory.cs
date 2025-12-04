using DominatorHouseCore.Enums;
using TumblrDominatorCore.Interface;

namespace TumblrDominatorCore.TumblrFactory
{
    public class TumblrUtilityFactory : ITumblrUtilityFactory
    {
        public ActivityType ModuleName { get; set; }

        public ITumblrReportFactory TumblrReportFactory { get; set; }

        public ITumblrViewCampaign TumblrViewCampaign { get; set; }
    }
}