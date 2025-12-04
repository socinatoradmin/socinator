using DominatorHouseCore.Enums;
using FaceDominatorCore.FDFactories;
using FaceDominatorCore.Interface;
using FaceDominatorCore.Utility;

namespace FaceDominatorUI.Utilities.Messanger.SendGreetingsToFriends
{
    public class PlaceScraperBaseFactory : IFdBaseFactory
    {
        public IFdUtilityFactory FdUtilityFactory()
        {
            var utilityFactory = new FdUtilityFactory();

            var builder = new FdBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.PlaceScraper)
                .AddReportFactory(new PlaceScraperReport())
                .AddViewCampaignFactory(new PlaceScraperViewCampaign());

            return builder.FdUtilityFactory;
        }
    }
}