using DominatorHouseCore.Enums;
using YoutubeDominatorCore.Interface;

namespace YoutubeDominatorCore.YDFactories
{
    public class YdUtilityFactory : IYdUtilityFactory
    {
        public ActivityType ModuleName { get; set; }

        public IYdReportFactory YdReportFactory { get; set; }

        public IYdViewCampaign YdViewCampaign { get; set; }
    }
}