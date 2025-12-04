using DominatorHouseCore.Enums;
using PinDominatorCore.Interface;
using PinDominatorCore.PDFactories;
using PinDominatorCore.Utility;

namespace PinDominator.Utility.PinScrap.User_Scraper
{
    public class UserScraperBaseFactory : IPdBaseFactory
    {
        public IPdUtilityFactory PdUtilityFactory()
        {
            var utilityFactory = new PdUtilityFactory();

            var builder = new PdBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.UserScraper)
                .AddReportFactory(new UserScraperReports())
                .AddViewCampaignFactory(new UserScraperViewCampaign());

            return builder.PdUtilityFactory;
        }
    }
}