using DominatorHouseCore.Enums;
using YoutubeDominatorCore.Interface;

namespace YoutubeDominatorCore.YDUtility
{
    public class YdBaseFactoryBuilder
    {
        public YdBaseFactoryBuilder(IYdUtilityFactory ydUtilityFactory)
        {
            YdUtilityFactory = ydUtilityFactory;
        }

        public IYdUtilityFactory YdUtilityFactory { get; set; }

        public YdBaseFactoryBuilder AddModuleName(ActivityType moduleName)
        {
            YdUtilityFactory.ModuleName = moduleName;
            return this;
        }

        public YdBaseFactoryBuilder AddReportFactory(IYdReportFactory reportFactory)
        {
            YdUtilityFactory.YdReportFactory = reportFactory;
            return this;
        }

        public YdBaseFactoryBuilder AddViewCampaignFactory(IYdViewCampaign fdViewCampaign)
        {
            YdUtilityFactory.YdViewCampaign = fdViewCampaign;
            return this;
        }
    }
}