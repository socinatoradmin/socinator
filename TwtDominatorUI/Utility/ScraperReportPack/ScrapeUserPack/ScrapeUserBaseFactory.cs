using DominatorHouseCore.Enums;
using TwtDominatorCore.Interface;
using TwtDominatorCore.TDFactories;
using TwtDominatorCore.TDUtility;

namespace TwtDominatorUI.Utility.ScraperReportPack.ScrapeUserPack
{
    public class ScrapeUserBaseFactory : ITDBaseFactory
    {
        public ITDUtilityFactory TDUtilityFactory()
        {
            var utilityFactory = new TDUtilityFactory();

            var builder = new TDBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.UserScraper)
                .AddReportFactory(new ScrapeUserReport())
                .AddViewCampaignFactory(new ScrapeUserViewCampaign());

            return builder.TDUtilityFactory;
        }
    }
}