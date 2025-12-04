using DominatorHouseCore.Enums;
using TwtDominatorCore.Interface;
using TwtDominatorCore.TDFactories;
using TwtDominatorCore.TDUtility;

namespace TwtDominatorUI.Utility.ScraperReportPack.ScrapeTweetPack
{
    public class ScrapeTweetBaseFactory : ITDBaseFactory
    {
        public ITDUtilityFactory TDUtilityFactory()
        {
            var utilityFactory = new TDUtilityFactory();

            var builder = new TDBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.TweetScraper)
                .AddReportFactory(new ScrapeTweetReport())
                .AddViewCampaignFactory(new ScrapeTweetViewCampaign());

            return builder.TDUtilityFactory;
        }
    }
}